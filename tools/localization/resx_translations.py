#!/usr/bin/env python3
"""Export/import WinForms .resx text for offline translation.

The CSV format is intentionally simple:
scope,key,zh-CN,en-US,fr-FR,ru-RU,comment
"""

from __future__ import annotations

import argparse
import csv
import re
import sys
import xml.etree.ElementTree as ET
from pathlib import Path


PROJECT_ROOT = Path(__file__).resolve().parents[2]
HMI_ROOT = PROJECT_ROOT / "HMI" / "JSZW1000A" / "JSZW1000A"
DEFAULT_OUT = PROJECT_ROOT / "文档" / "localization" / "jszw1000a-texts.csv"
RESX_NAME_RE = re.compile(r"^(?P<scope>.+?)(?:\.(?P<culture>[a-z]{2}(?:-[A-Z]{2})?))?\.resx$")
TEXT_SUFFIXES = (".Text", ".HeaderText")
IGNORED_REPORT_KEYS = {
    ("MainFrm", "richMsgInfo.Text"),
    ("SubWindows/SubOPAutoView", "richMsgInfo.Text"),
}


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Export/import JSZW1000A .resx translations.")
    subparsers = parser.add_subparsers(dest="command", required=True)

    export_parser = subparsers.add_parser("export", help="Export current .resx text into CSV.")
    export_parser.add_argument("--hmi-root", type=Path, default=HMI_ROOT)
    export_parser.add_argument("--out", type=Path, default=DEFAULT_OUT)
    export_parser.add_argument("--cultures", default="zh-CN,en-US,fr-FR,ru-RU")

    report_parser = subparsers.add_parser("report", help="Report missing translations from the CSV.")
    report_parser.add_argument("--csv", type=Path, default=DEFAULT_OUT)
    report_parser.add_argument("--cultures", default="fr-FR,ru-RU")

    import_parser = subparsers.add_parser("import", help="Import translated CSV into target .resx files.")
    import_parser.add_argument("--hmi-root", type=Path, default=HMI_ROOT)
    import_parser.add_argument("--csv", type=Path, default=DEFAULT_OUT)
    import_parser.add_argument("--cultures", default="fr-FR,ru-RU")

    return parser.parse_args()


def culture_columns(raw: str) -> list[str]:
    return [part.strip() for part in raw.split(",") if part.strip()]


def culture_from_path(hmi_root: Path, path: Path) -> tuple[str, str] | None:
    match = RESX_NAME_RE.match(path.name)
    if not match:
        return None
    relative_parent = path.parent.relative_to(hmi_root)
    base_scope = match.group("scope")
    scope = str(relative_parent / base_scope) if str(relative_parent) != "." else base_scope
    culture = match.group("culture") or "zh-CN"
    return scope.replace("\\", "/"), culture


def iter_resx_files(hmi_root: Path) -> list[Path]:
    return sorted(
        path
        for path in hmi_root.rglob("*.resx")
        if "\\bin\\" not in str(path) and "\\obj\\" not in str(path)
    )


def read_xml(path: Path) -> ET.ElementTree:
    return ET.parse(path)


def data_entries(path: Path, scope: str) -> dict[str, tuple[str, str]]:
    tree = read_xml(path)
    entries: dict[str, tuple[str, str]] = {}
    for data in tree.getroot().findall("data"):
        name = data.attrib.get("name", "")
        value_node = data.find("value")
        if value_node is None:
            continue
        if not is_translatable_key(scope, name):
            continue
        comment_node = data.find("comment")
        entries[name] = (value_node.text or "", comment_node.text if comment_node is not None and comment_node.text else "")
    return entries


def is_translatable_key(scope: str, key: str) -> bool:
    if not key:
        return False
    if key.startswith(">>"):
        return False
    if scope == "Strings":
        return True
    return key == "$this.Text" or key.endswith(TEXT_SUFFIXES)


def should_report_missing(row: dict[str, str], culture: str) -> bool:
    if (row["scope"], row["key"]) in IGNORED_REPORT_KEYS:
        return False
    if not row.get("zh-CN", "").strip() and not row.get("en-US", "").strip():
        return False
    return not row.get(culture, "").strip()


def localized_resx_path(hmi_root: Path, scope: str, culture: str) -> Path:
    base = hmi_root / f"{scope}.resx"
    if culture == "zh-CN":
        return base
    return hmi_root / f"{scope}.{culture}.resx"


def collect(hmi_root: Path, cultures: list[str]) -> dict[tuple[str, str], dict[str, str]]:
    rows: dict[tuple[str, str], dict[str, str]] = {}
    comments: dict[tuple[str, str], str] = {}

    for path in iter_resx_files(hmi_root):
        parsed = culture_from_path(hmi_root, path)
        if parsed is None:
            continue
        scope, culture = parsed
        if culture not in cultures:
            continue
        for key, (value, comment) in data_entries(path, scope).items():
            row_key = (scope, key)
            rows.setdefault(row_key, {})[culture] = value
            if comment:
                comments[row_key] = comment

    for row_key, comment in comments.items():
        rows.setdefault(row_key, {})["comment"] = comment

    return rows


def export_csv(hmi_root: Path, out_path: Path, cultures: list[str]) -> None:
    rows = collect(hmi_root, cultures)
    out_path.parent.mkdir(parents=True, exist_ok=True)
    with out_path.open("w", encoding="utf-8-sig", newline="") as handle:
        writer = csv.DictWriter(handle, fieldnames=["scope", "key", *cultures, "comment"])
        writer.writeheader()
        for (scope, key), values in sorted(rows.items()):
            row = {"scope": scope, "key": key, "comment": values.get("comment", "")}
            for culture in cultures:
                row[culture] = values.get(culture, "")
            writer.writerow(row)


def ensure_target_tree(source_path: Path, target_path: Path) -> ET.ElementTree:
    if target_path.exists():
        return read_xml(target_path)
    target_path.parent.mkdir(parents=True, exist_ok=True)
    return read_xml(source_path)


def import_csv(hmi_root: Path, csv_path: Path, target_cultures: list[str]) -> None:
    source_by_scope: dict[str, Path] = {}
    values: dict[tuple[str, str], dict[str, str]] = {}

    with csv_path.open("r", encoding="utf-8-sig", newline="") as handle:
        for row in csv.DictReader(handle):
            scope = row["scope"]
            key = row["key"]
            source_by_scope.setdefault(scope, localized_resx_path(hmi_root, scope, "zh-CN"))
            values[(scope, key)] = row

    for culture in target_cultures:
        by_scope: dict[str, dict[str, str]] = {}
        for (scope, key), row in values.items():
            text = row.get(culture, "")
            if text:
                by_scope.setdefault(scope, {})[key] = text

        for scope, translations in by_scope.items():
            source_path = source_by_scope[scope]
            if not source_path.exists():
                print(f"skip missing source: {source_path}", file=sys.stderr)
                continue
            target_path = localized_resx_path(hmi_root, scope, culture)
            tree = ensure_target_tree(source_path, target_path)
            root = tree.getroot()
            existing = {node.attrib.get("name", ""): node for node in root.findall("data")}

            for key, text in translations.items():
                node = existing.get(key)
                if node is None:
                    node = ET.SubElement(root, "data", {"name": key, "{http://www.w3.org/XML/1998/namespace}space": "preserve"})
                    ET.SubElement(node, "value")
                value_node = node.find("value")
                if value_node is None:
                    value_node = ET.SubElement(node, "value")
                value_node.text = text

            ET.indent(tree, space="  ")
            tree.write(target_path, encoding="utf-8", xml_declaration=True)


def report_missing(csv_path: Path, target_cultures: list[str]) -> int:
    missing: dict[str, list[str]] = {culture: [] for culture in target_cultures}
    with csv_path.open("r", encoding="utf-8-sig", newline="") as handle:
        for row in csv.DictReader(handle):
            row_id = f"{row['scope']}::{row['key']}"
            for culture in target_cultures:
                if should_report_missing(row, culture):
                    missing[culture].append(row_id)

    total = 0
    for culture in target_cultures:
        count = len(missing[culture])
        total += count
        print(f"{culture}: {count} missing")
        for row_id in missing[culture][:20]:
            print(f"  {row_id}")
        if count > 20:
            print(f"  ... {count - 20} more")
    return total


def main() -> int:
    args = parse_args()
    if args.command == "export":
        export_csv(args.hmi_root, args.out, culture_columns(args.cultures))
        print(args.out)
        return 0
    if args.command == "report":
        return 1 if report_missing(args.csv, culture_columns(args.cultures)) else 0
    if args.command == "import":
        import_csv(args.hmi_root, args.csv, culture_columns(args.cultures))
        return 0
    return 1


if __name__ == "__main__":
    raise SystemExit(main())

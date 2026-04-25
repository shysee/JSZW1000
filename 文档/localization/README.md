# JSZW1000A 多语言文本导出导入链路

## 目标

把现有 `Strings*.resx` 和窗体 `.resx` 里的可翻译文本导出成 CSV，交给人工或翻译工具处理后，再整体导入生成目标语言资源表。

## 导出

```powershell
$env:PYTHONUTF8='1'
python tools\localization\resx_translations.py export
```

默认输出：

```text
文档\localization\jszw1000a-texts.csv
```

CSV 列：

```text
scope,key,zh-CN,en-US,fr-FR,ru-RU,comment
```

翻译时只填写 `fr-FR`、`ru-RU` 列，不要改 `scope` 和 `key`。

## 缺失检查

```powershell
$env:PYTHONUTF8='1'
python tools\localization\resx_translations.py report
```

该命令会列出 `fr-FR`、`ru-RU` 仍为空的资源键。

## 导入

```powershell
$env:PYTHONUTF8='1'
python tools\localization\resx_translations.py import
```

导入后会生成或更新：

```text
HMI\JSZW1000A\JSZW1000A\Strings.fr-FR.resx
HMI\JSZW1000A\JSZW1000A\Strings.ru-RU.resx
HMI\JSZW1000A\JSZW1000A\SubWindows\*.fr-FR.resx
HMI\JSZW1000A\JSZW1000A\SubWindows\*.ru-RU.resx
```

## 后续接入点

资源表导入后，还需要把运行时语言枚举和切换入口扩展到 `fr-FR`、`ru-RU`，并对主要页面做布局回归，尤其是按钮、窄标签、表格列头和多行文本。

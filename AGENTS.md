# JSZW400 Agent Notes

- For WinForms Designer issues, preserve a complete designer-openable program. Do not rename, move, delete, or hide `.Designer.cs`, `.resx`, `.csproj.user`, `.sln`, `.csproj`, or `.vs` state as a workaround unless the user explicitly approves that exact action.
- When debugging Designer failures, fix the root cause in place and keep the project usable in Visual Studio. A successful `dotnet build` alone is not enough if the requested outcome is opening the design surface.

## Agent skills

### Issue tracker

Issues and PRDs for this repo are tracked as local markdown files under `.scratch/`. See `docs/agents/issue-tracker.md`.

### Triage labels

Use the default five triage labels: `needs-triage`, `needs-info`, `ready-for-agent`, `ready-for-human`, `wontfix`. See `docs/agents/triage-labels.md`.

### Domain docs

This repo is configured as a single-context repo. Skills should read the root `CONTEXT.md` and `docs/adr/` when present. See `docs/agents/domain.md`.

# assettrack

> An offline-first asset tracking service for disconnected and access-restricted environments.

`assettrack` records and tracks physical or logical assets entirely within a
local boundary. It stores data in a local SQLite database, reconciles changes
across disconnected nodes using deterministic conflict resolution, and captures
every mutation in an append-only audit log. It is designed for environments
where network connectivity is intermittent or restricted and where data must
not leave the local boundary.

## The Problem

In logistics and defense settings, asset records must be maintained at sites
that are frequently offline or air-gapped, then reconciled when a link becomes
available — without losing data and without an authoritative central server
always being reachable. `assettrack` treats offline as the default state and
sync as an explicit, auditable event.

## Architecture

```
AssetTrack.Core    pure domain + conflict/sync logic, zero I/O (fully unit-tested)
AssetTrack.Data    SQLite persistence + append-only audit log (EF Core)
AssetTrack.Sync    transport abstraction + file-based offline reconciliation
AssetTrack.App     runnable host: DI wiring + commands
```

See [`docs/architecture.md`](docs/architecture.md) and the decision records in
[`docs/adr/`](docs/adr/) for rationale.

## Requirements

- .NET 8 SDK (LTS)

## Build & Run

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/AssetTrack.App -- list
```

## Installer

A Windows MSI is produced automatically by CI on tagged releases (see
[`.github/workflows/release.yml`](.github/workflows/release.yml)). MSI is a
Windows-only package format, so it is built on a Windows runner rather than
locally on Linux.

## License

MIT — see [LICENSE](LICENSE).

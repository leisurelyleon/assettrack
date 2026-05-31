# Architecture

`assettrack` is a .NET 8 solution organized so that all domain logic is pure and
independently testable, with I/O confined to dedicated layers.

## Project responsibilities

- **AssetTrack.Core** — Pure domain: models, abstractions, and the conflict
  resolution and reconciliation logic. No database, file system, or network
  access. Carries the majority of the unit tests.
- **AssetTrack.Data** — SQLite persistence via EF Core, including the
  repository and the append-only audit log.
- **AssetTrack.Sync** — Transport abstraction and the sync engine. A file-based
  transport supports offline reconciliation; the abstraction allows other
  channels without changing sync logic.
- **AssetTrack.App** — The runnable host: dependency-injection wiring and the
  `add`, `list`, and `sync` commands.

## Dependency direction

```text
App ──► Sync ──► Core
│  │
└──► Data ──► Core
```

Dependencies flow inward toward `Core`, which depends on nothing. This is what
keeps the conflict and reconciliation logic testable in isolation.

## Offline-first model

Offline is the default state. Local writes always succeed against the local
SQLite store. Synchronization is an explicit, auditable event: the engine pulls
remote state, reconciles deterministically, persists the merged result, and
publishes it back — and safely no-ops when no transport is available.

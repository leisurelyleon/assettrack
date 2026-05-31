# 1. Offline-first with local SQLite

- Status: Accepted
- Date: 2026-05

## Context

The target environments are frequently disconnected or air-gapped. A design
that assumes an always-reachable central database is unusable there.

## Decision

Treat each node as authoritative for its own local writes, stored in a local
SQLite database. Synchronization is an explicit, occasional event rather than a
continuous requirement.

## Consequences

- Local operations always succeed regardless of connectivity.
- A reconciliation step is required to converge replicas.
- SQLite keeps the storage footprint small and dependency-free, suitable for
  constrained deployments.

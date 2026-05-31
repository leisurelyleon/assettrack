# 2. Deterministic conflict resolution

- Status: Accepted
- Date: 2026-05

## Context

When disconnected replicas diverge, the same conflict may be resolved
independently on multiple nodes. If resolution is not deterministic, replicas
can permanently disagree.

## Decision

Resolve conflicts with a fixed rule order: higher version wins; then more recent
modification time; then a node-independent tiebreak on a content signature. The
same two inputs always yield the same winner on any node.

## Consequences

- Replicas converge to identical state without coordination.
- Resolution is fully unit-testable with no infrastructure.
- The rules are explicit and auditable, which suits regulated environments.

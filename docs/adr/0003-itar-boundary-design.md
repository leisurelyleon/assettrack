# 3. Keeping data within the local boundary

- Status: Accepted
- Date: 2026-05

## Context

In export-controlled (e.g. ITAR) and other regulated settings, asset data must
not leave a defined boundary without explicit, controlled action.

## Decision

No component transmits data automatically. The default file-based transport
exchanges data only through an explicit local file, so data leaves the boundary
only if that file is deliberately moved. No telemetry or external calls are made.

## Consequences

- The system is safe to run in air-gapped environments by default.
- Any data egress is an explicit, observable action by an operator.
- Future network transports must preserve this explicit-action principle.

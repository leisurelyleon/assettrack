#!/usr/bin/env bash
# Convenience wrapper to run the AssetTrack app with any passed arguments.
#
# Examples:
#   ./scripts/run.sh add A-001 "Pallet Jack" Warehouse-3 Equipment
#   ./scripts/run.sh list
#   ./scripts/run.sh sync
set -euo pipefail

dotnet run --project src/AssetTrack.App -- "$@"

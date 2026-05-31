#!/usr/bin/env bash
# Build the full solution in Release configuration.
set -euo pipefail

echo "Restoring and building AssetTrack (Release)..."
dotnet build AssetTrack.sln --configuration Release
echo "Build complete."

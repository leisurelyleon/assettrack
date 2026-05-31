namespace AssetTrack.Core.Sync;

using AssetTrack.Core.Models;

/// <summary>
/// Deterministic, node-independent conflict resolution. The same two inputs
/// always yield the same winner on any node, which is essential for consistent
/// reconciliation across disconnected replicas.
/// </summary>
public sealed class ConflictResolver
{
    public ConflictResolution Resolve(Asset local, Asset remote)
    {
        if (local.Id != remote.Id)
        {
            throw new ArgumentException("Cannot resolve a conflict between assets with different ids.");
        }

        if (AreContentEqual(local, remote))
        {
            return new ConflictResolution(ConflictOutcome.NoConflict, local, "Local and remote are identical.");
        }

        // Rule 1: the higher version wins.
        if (local.Version != remote.Version)
        {
            return local.Version > remote.Version
                ? new ConflictResolution(ConflictOutcome.LocalWins, local, "Local has a higher version.")
                : new ConflictResolution(ConflictOutcome.RemoteWins, remote, "Remote has a higher version.");
        }

        // Rule 2: equal version -> the more recent modification wins.
        if (local.LastModifiedUtc != remote.LastModifiedUtc)
        {
            return local.LastModifiedUtc > remote.LastModifiedUtc
                ? new ConflictResolution(ConflictOutcome.LocalWins, local, "Local was modified more recently.")
                : new ConflictResolution(ConflictOutcome.RemoteWins, remote, "Remote was modified more recently.");
        }

        // Rule 3: deterministic, node-independent tiebreak on a content signature.
        int comparison = string.CompareOrdinal(Signature(local), Signature(remote));
        return comparison >= 0
            ? new ConflictResolution(ConflictOutcome.LocalWins, local, "Tiebreak by content signature.")
            : new ConflictResolution(ConflictOutcome.RemoteWins, remote, "Tiebreak by content signature.");
    }

    private static bool AreContentEqual(Asset a, Asset b) =>
        a.Name == b.Name
        && a.Location == b.Location
        && a.Category == b.Category
        && a.IsDecommissioned == b.IsDecommissioned
        && a.Version == b.Version
        && a.LastModifiedUtc == b.LastModifiedUtc;

    private static string Signature(Asset a) =>
        $"{a.Name}|{a.Location}|{a.Category}|{a.IsDecommissioned}";
}

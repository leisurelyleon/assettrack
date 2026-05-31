namespace AssetTrack.Core.Sync;

using AssetTrack.Core.Models;

/// <summary>The merged result of reconciling local and remote asset sets.</summary>
public sealed class ReconciliationResult
{
    public IReadOnlyList<Asset> Merged { get; }
    public IReadOnlyList<ConflictResolution> Conflicts { get; }

    public ReconciliationResult(IReadOnlyList<Asset> merged, IReadOnlyList<ConflictResolution> conflicts)
    {
        Merged = merged;
        Conflicts = conflicts;
    }
}

/// <summary>
/// Merges a local and a remote set of assets, resolving any per-asset conflicts
/// deterministically and adopting assets that exist on only one side.
/// </summary>
public sealed class SyncReconciler
{
    private readonly ConflictResolver _resolver;

    public SyncReconciler(ConflictResolver resolver)
    {
        _resolver = resolver;
    }

    public ReconciliationResult Reconcile(IEnumerable<Asset> local, IEnumerable<Asset> remote)
    {
        var localById = local.ToDictionary(a => a.Id);
        var merged = new Dictionary<string, Asset>(localById);
        var conflicts = new List<ConflictResolution>();

        foreach (var remoteAsset in remote)
        {
            if (localById.TryGetValue(remoteAsset.Id, out var localAsset))
            {
                var resolution = _resolver.Resolve(localAsset, remoteAsset);
                merged[remoteAsset.Id] = resolution.Winner;

                if (resolution.Outcome != ConflictOutcome.NoConflict)
                {
                    conflicts.Add(resolution);
                }
            }
            else
            {
                merged[remoteAsset.Id] = remoteAsset;
            }
        }

        var mergedList = merged.Values
            .OrderBy(a => a.Id, StringComparer.Ordinal)
            .ToList();

        return new ReconciliationResult(mergedList, conflicts);
    }
}

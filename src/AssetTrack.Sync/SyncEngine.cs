namespace AssetTrack.Sync;

using AssetTrack.Core.Abstractions;
using AssetTrack.Core.Models;
using AssetTrack.Core.Sync;

/// <summary>A summary of a sync attempt.</summary>
public sealed class SyncSummary
{
    public bool WasCompleted { get; }
    public int MergedCount { get; }
    public int ConflictCount { get; }
    public DateTimeOffset? CompletedAtUtc { get; }
    public string Message { get; }

    private SyncSummary(bool completed, int merged, int conflicts, DateTimeOffset? at, string message)
    {
        WasCompleted = completed;
        MergedCount = merged;
        ConflictCount = conflicts;
        CompletedAtUtc = at;
        Message = message;
    }

    public static SyncSummary Completed(int mergedCount, int conflictCount, DateTimeOffset at) =>
        new(true, mergedCount, conflictCount, at, $"Synced {mergedCount} asset(s); {conflictCount} conflict(s) resolved.");

    public static SyncSummary Skipped(string reason) =>
        new(false, 0, 0, null, reason);
}

/// <summary>
/// Orchestrates a reconciliation cycle: pull remote state, merge it with local
/// state via deterministic resolution, persist the result, and publish it back.
/// </summary>
public sealed class SyncEngine
{
    private readonly IAssetRepository _repository;
    private readonly ISyncTransport _transport;
    private readonly SyncReconciler _reconciler;
    private readonly IClock _clock;

    public SyncEngine(IAssetRepository repository, ISyncTransport transport, SyncReconciler reconciler, IClock clock)
    {
        _repository = repository;
        _transport = transport;
        _reconciler = reconciler;
        _clock = clock;
    }

    public async Task<SyncSummary> SyncAsync(CancellationToken cancellationToken = default)
    {
        if (!await _transport.IsAvailableAsync(cancellationToken))
        {
            return SyncSummary.Skipped("Transport unavailable; remaining offline.");
        }

        var local = await _repository.GetAllAsync(cancellationToken);
        var remote = await _transport.ReceiveAsync(cancellationToken);

        var result = _reconciler.Reconcile(local, remote);

        foreach (var asset in result.Merged)
        {
            asset.SyncState = SyncState.Synced;
            await _repository.UpsertAsync(asset, cancellationToken);
        }

        await _transport.SendAsync(result.Merged, cancellationToken);

        return SyncSummary.Completed(result.Merged.Count, result.Conflicts.Count, _clock.UtcNow);
    }
}

namespace AssetTrack.Core.Tests;

using AssetTrack.Core.Models;
using AssetTrack.Core.Sync;
using Xunit;

public sealed class SyncReconcilerTests
{
    private static Asset MakeAsset(string id, long version, string name) =>
        new()
        {
            Id = id,
            Name = name,
            Location = "Bay-1",
            Category = "Tools",
            Version = version,
            LastModifiedUtc = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            SyncState = SyncState.Local
        };

    private readonly SyncReconciler _reconciler = new(new ConflictResolver());

    [Fact]
    public void RemoteOnlyAsset_IsAdopted()
    {
        var local = new[] { MakeAsset("A-1", 1, "Local") };
        var remote = new[] { MakeAsset("A-2", 1, "Remote") };

        var result = _reconciler.Reconcile(local, remote);

        Assert.Equal(2, result.Merged.Count);
        Assert.Contains(result.Merged, a => a.Id == "A-2");
    }

    [Fact]
    public void ConflictingAsset_ResolvesToHigherVersion_AndIsRecorded()
    {
        var local = new[] { MakeAsset("A-1", 1, "Local") };
        var remote = new[] { MakeAsset("A-1", 4, "Remote") };

        var result = _reconciler.Reconcile(local, remote);

        Assert.Single(result.Merged);
        Assert.Equal(4, result.Merged[0].Version);
        Assert.Single(result.Conflicts);
        Assert.Equal(ConflictOutcome.RemoteWins, result.Conflicts[0].Outcome);
    }

    [Fact]
    public void IdenticalAsset_MergesWithoutRecordingConflict()
    {
        var local = new[] { MakeAsset("A-1", 2, "Same") };
        var remote = new[] { MakeAsset("A-1", 2, "Same") };

        var result = _reconciler.Reconcile(local, remote);

        Assert.Single(result.Merged);
        Assert.Empty(result.Conflicts);
    }

    [Fact]
    public void MergedResult_IsOrderedById()
    {
        var local = new[] { MakeAsset("A-3", 1, "C"), MakeAsset("A-1", 1, "A") };
        var remote = new[] { MakeAsset("A-2", 1, "B") };

        var result = _reconciler.Reconcile(local, remote);

        Assert.Equal(new[] { "A-1", "A-2", "A-3" }, result.Merged.Select(a => a.Id).ToArray());
    }
}

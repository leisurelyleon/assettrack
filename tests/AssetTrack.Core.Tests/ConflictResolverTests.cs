namespace AssetTrack.Core.Tests;

using AssetTrack.Core.Models;
using AssetTrack.Core.Sync;
using Xunit;

public sealed class ConflictResolverTests
{
    private static Asset MakeAsset(
        string id = "A-1",
        string name = "Widget",
        long version = 1,
        string? location = "Bay-1",
        DateTimeOffset? modified = null) =>
        new()
        {
            Id = id,
            Name = name,
            Location = location,
            Category = "Tools",
            Version = version,
            LastModifiedUtc = modified ?? new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            SyncState = SyncState.Local
        };

    private readonly ConflictResolver _resolver = new();

    [Fact]
    public void IdenticalAssets_ProduceNoConflict()
    {
        var a = MakeAsset();
        var b = MakeAsset();

        var result = _resolver.Resolve(a, b);

        Assert.Equal(ConflictOutcome.NoConflict, result.Outcome);
    }

    [Fact]
    public void HigherVersion_Wins()
    {
        var local = MakeAsset(version: 5, name: "New");
        var remote = MakeAsset(version: 3, name: "Old");

        var result = _resolver.Resolve(local, remote);

        Assert.Equal(ConflictOutcome.LocalWins, result.Outcome);
        Assert.Equal(5, result.Winner.Version);
    }

    [Fact]
    public void EqualVersion_MoreRecentModificationWins()
    {
        var older = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var newer = new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero);

        var local = MakeAsset(version: 2, name: "Local", modified: older);
        var remote = MakeAsset(version: 2, name: "Remote", modified: newer);

        var result = _resolver.Resolve(local, remote);

        Assert.Equal(ConflictOutcome.RemoteWins, result.Outcome);
    }

    [Fact]
    public void FullTie_ResolvesDeterministically_RegardlessOfArgumentOrder()
    {
        var sameTime = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero);
        var local = MakeAsset(version: 1, name: "Alpha", location: "Bay-1", modified: sameTime);
        var remote = MakeAsset(version: 1, name: "Beta", location: "Bay-2", modified: sameTime);

        var forward = _resolver.Resolve(local, remote);
        var reversed = _resolver.Resolve(remote, local);

        // The same winning *content* must be chosen no matter the order — the
        // property that guarantees replicas converge.
        Assert.Equal(forward.Winner.Name, reversed.Winner.Name);
    }

    [Fact]
    public void DifferentIds_Throws()
    {
        var local = MakeAsset(id: "A-1");
        var remote = MakeAsset(id: "A-2");

        Assert.Throws<ArgumentException>(() => _resolver.Resolve(local, remote));
    }
}

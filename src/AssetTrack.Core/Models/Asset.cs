namespace AssetTrack.Core.Models;

/// <summary>
/// A tracked asset. Non-nullable string members are initialized to empty so the
/// type is safe to materialize by EF Core and serializers without nullability
/// warnings, while still carrying meaningful values in normal use.
/// </summary>
public sealed class Asset
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Category { get; set; }
    public bool IsDecommissioned { get; set; }

    /// <summary>Monotonic version, incremented on every local mutation.</summary>
    public long Version { get; set; }

    public DateTimeOffset LastModifiedUtc { get; set; }
    public SyncState SyncState { get; set; } = SyncState.Local;
}

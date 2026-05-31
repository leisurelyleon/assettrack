namespace AssetTrack.Core.Models;

/// <summary>
/// A self-contained description of a change to an asset, suitable for transport
/// between disconnected nodes. Part of the domain model for change-based sync.
/// </summary>
public sealed class AssetChange
{
    public string AssetId { get; set; } = string.Empty;
    public ChangeKind Kind { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Category { get; set; }
    public long Version { get; set; }
    public DateTimeOffset TimestampUtc { get; set; }
    public string OriginNode { get; set; } = string.Empty;
}

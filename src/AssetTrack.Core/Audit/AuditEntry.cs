namespace AssetTrack.Core.Audit;

using AssetTrack.Core.Models;

/// <summary>A single append-only audit record. Sequence is assigned by storage.</summary>
public sealed class AuditEntry
{
    public long Sequence { get; set; }
    public string AssetId { get; set; } = string.Empty;
    public ChangeKind Kind { get; set; }
    public DateTimeOffset TimestampUtc { get; set; }
    public string Detail { get; set; } = string.Empty;
}

namespace AssetTrack.Core.Models;

/// <summary>The kind of mutation represented by a change or audit entry.</summary>
public enum ChangeKind
{
    Created,
    Updated,
    Decommissioned
}

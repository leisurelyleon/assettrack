namespace AssetTrack.Core.Models;

/// <summary>The synchronization state of a locally held asset.</summary>
public enum SyncState
{
    Local,
    Synced,
    Conflicted
}

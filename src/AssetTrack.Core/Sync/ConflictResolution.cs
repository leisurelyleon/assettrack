namespace AssetTrack.Core.Sync;

using AssetTrack.Core.Models;

/// <summary>The outcome of resolving a conflict between two asset versions.</summary>
public enum ConflictOutcome
{
    NoConflict,
    LocalWins,
    RemoteWins
}

/// <summary>The result of a conflict resolution, including the winning asset.</summary>
public sealed class ConflictResolution
{
    public ConflictOutcome Outcome { get; }
    public Asset Winner { get; }
    public string Reason { get; }

    public ConflictResolution(ConflictOutcome outcome, Asset winner, string reason)
    {
        Outcome = outcome;
        Winner = winner;
        Reason = reason;
    }
}

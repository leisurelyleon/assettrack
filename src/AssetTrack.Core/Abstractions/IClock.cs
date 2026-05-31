namespace AssetTrack.Core.Abstractions;

/// <summary>An injectable clock, enabling deterministic time in tests.</summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

namespace AssetTrack.Sync.Transport;

/// <summary>
/// Determines connectivity via an injected probe, keeping the offline decision
/// testable and independent of any specific networking implementation.
/// </summary>
public sealed class OfflineDetector
{
    private readonly Func<bool> _connectivityProbe;

    public OfflineDetector(Func<bool> connectivityProbe)
    {
        _connectivityProbe = connectivityProbe;
    }

    public bool IsOffline() => !_connectivityProbe();
}

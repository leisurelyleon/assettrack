namespace AssetTrack.Sync;

using AssetTrack.Core.Models;

/// <summary>
/// Abstraction over the channel assets are exchanged through. Swapping the
/// implementation (file, network, removable media) does not change sync logic.
/// </summary>
public interface ISyncTransport
{
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Asset>> ReceiveAsync(CancellationToken cancellationToken = default);

    Task SendAsync(IReadOnlyList<Asset> assets, CancellationToken cancellationToken = default);
}

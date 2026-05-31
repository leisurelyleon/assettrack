namespace AssetTrack.Core.Abstractions;

using AssetTrack.Core.Models;

/// <summary>Persistence abstraction for assets. Implemented by the data layer.</summary>
public interface IAssetRepository
{
    Task<Asset?> GetAsync(string id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Asset>> GetAllAsync(CancellationToken cancellationToken = default);

    Task UpsertAsync(Asset asset, CancellationToken cancellationToken = default);
}

namespace AssetTrack.Data.Repositories;

using AssetTrack.Core.Abstractions;
using AssetTrack.Core.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>SQLite-backed implementation of <see cref="IAssetRepository"/>.</summary>
public sealed class SqliteAssetRepository : IAssetRepository
{
    private readonly AssetDbContext _db;

    public SqliteAssetRepository(AssetDbContext db)
    {
        _db = db;
    }

    public async Task<Asset?> GetAsync(string id, CancellationToken cancellationToken = default) =>
        await _db.Assets.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Asset>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _db.Assets.AsNoTracking().OrderBy(a => a.Id).ToListAsync(cancellationToken);

    public async Task UpsertAsync(Asset asset, CancellationToken cancellationToken = default)
    {
        var existing = await _db.Assets.FirstOrDefaultAsync(a => a.Id == asset.Id, cancellationToken);

        if (existing is null)
        {
            _db.Assets.Add(asset);
        }
        else
        {
            existing.Name = asset.Name;
            existing.Location = asset.Location;
            existing.Category = asset.Category;
            existing.IsDecommissioned = asset.IsDecommissioned;
            existing.Version = asset.Version;
            existing.LastModifiedUtc = asset.LastModifiedUtc;
            existing.SyncState = asset.SyncState;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}

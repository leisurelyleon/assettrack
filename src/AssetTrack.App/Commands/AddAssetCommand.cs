namespace AssetTrack.App.Commands;

using AssetTrack.Core.Abstractions;
using AssetTrack.Core.Audit;
using AssetTrack.Core.Models;

/// <summary>Adds a new asset or updates an existing one, with an audit entry.</summary>
public sealed class AddAssetCommand
{
    private readonly IAssetRepository _repository;
    private readonly IAuditLog _auditLog;
    private readonly IClock _clock;

    public AddAssetCommand(IAssetRepository repository, IAuditLog auditLog, IClock clock)
    {
        _repository = repository;
        _auditLog = auditLog;
        _clock = clock;
    }

    public async Task ExecuteAsync(string id, string name, string? location, string? category)
    {
        var now = _clock.UtcNow;
        var existing = await _repository.GetAsync(id);
        var kind = existing is null ? ChangeKind.Created : ChangeKind.Updated;

        var asset = new Asset
        {
            Id = id,
            Name = name,
            Location = location,
            Category = category,
            IsDecommissioned = false,
            Version = (existing?.Version ?? 0) + 1,
            LastModifiedUtc = now,
            SyncState = SyncState.Local
        };

        await _repository.UpsertAsync(asset);
        await _auditLog.AppendAsync(new AuditEntry
        {
            AssetId = id,
            Kind = kind,
            TimestampUtc = now,
            Detail = $"{kind} asset '{name}' (v{asset.Version})."
        });

        Console.WriteLine($"{kind}: {id} — {name} (v{asset.Version})");
    }
}

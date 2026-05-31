namespace AssetTrack.Core.Abstractions;

using AssetTrack.Core.Audit;

/// <summary>Append-only audit log abstraction. Entries are never modified.</summary>
public interface IAuditLog
{
    Task AppendAsync(AuditEntry entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AuditEntry>> GetEntriesAsync(CancellationToken cancellationToken = default);
}

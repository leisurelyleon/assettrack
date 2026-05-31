namespace AssetTrack.Data.Audit;

using AssetTrack.Core.Abstractions;
using AssetTrack.Core.Audit;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// SQLite-backed append-only audit log. Entries are only ever inserted and
/// read in sequence order; no update or delete path exists.
/// </summary>
public sealed class SqliteAuditLog : IAuditLog
{
    private readonly AssetDbContext _db;

    public SqliteAuditLog(AssetDbContext db)
    {
        _db = db;
    }

    public async Task AppendAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        _db.AuditEntries.Add(entry);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AuditEntry>> GetEntriesAsync(CancellationToken cancellationToken = default) =>
        await _db.AuditEntries.AsNoTracking().OrderBy(e => e.Sequence).ToListAsync(cancellationToken);
}

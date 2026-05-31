namespace AssetTrack.Data;

using AssetTrack.Core.Audit;
using AssetTrack.Core.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>EF Core context backing the local SQLite store.</summary>
public sealed class AssetDbContext : DbContext
{
    public AssetDbContext(DbContextOptions<AssetDbContext> options)
        : base(options)
    {
    }

    public DbSet<Asset> Assets => Set<Asset>();

    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Id).IsRequired();
            entity.Property(a => a.Name).IsRequired();
        });

        modelBuilder.Entity<AuditEntry>(entity =>
        {
            entity.HasKey(a => a.Sequence);
            entity.Property(a => a.Sequence).ValueGeneratedOnAdd();
            entity.Property(a => a.AssetId).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}

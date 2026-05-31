namespace AssetTrack.Data.Tests;

using AssetTrack.Core.Models;
using AssetTrack.Data;
using AssetTrack.Data.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

/// <summary>
/// Repository tests against a real SQLite engine running in-memory. A single
/// connection is held open for the lifetime of the test, because an in-memory
/// SQLite database is discarded as soon as its last connection closes.
/// </summary>
public sealed class SqliteAssetRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AssetDbContext> _options;

    public SqliteAssetRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<AssetDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new AssetDbContext(_options);
        context.Database.EnsureCreated();
    }

    private AssetDbContext NewContext() => new(_options);

    private static Asset MakeAsset(string id = "A-1", long version = 1, string name = "Widget") =>
        new()
        {
            Id = id,
            Name = name,
            Location = "Bay-1",
            Category = "Tools",
            Version = version,
            LastModifiedUtc = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            SyncState = SyncState.Local
        };

    [Fact]
    public async Task Upsert_InsertsNewAsset()
    {
        await using (var ctx = NewContext())
        {
            var repo = new SqliteAssetRepository(ctx);
            await repo.UpsertAsync(MakeAsset());
        }

        await using (var ctx = NewContext())
        {
            var repo = new SqliteAssetRepository(ctx);
            var fetched = await repo.GetAsync("A-1");
            Assert.NotNull(fetched);
            Assert.Equal("Widget", fetched!.Name);
        }
    }

    [Fact]
    public async Task Upsert_UpdatesExistingAsset()
    {
        await using (var ctx = NewContext())
        {
            await new SqliteAssetRepository(ctx).UpsertAsync(MakeAsset(version: 1, name: "Old"));
        }

        await using (var ctx = NewContext())
        {
            await new SqliteAssetRepository(ctx).UpsertAsync(MakeAsset(version: 2, name: "New"));
        }

        await using (var ctx = NewContext())
        {
            var fetched = await new SqliteAssetRepository(ctx).GetAsync("A-1");
            Assert.NotNull(fetched);
            Assert.Equal("New", fetched!.Name);
            Assert.Equal(2, fetched.Version);
        }

        // An update must not create a second row.
        await using (var verify = NewContext())
        {
            Assert.Equal(1, await verify.Assets.CountAsync());
        }
    }

    [Fact]
    public async Task GetAll_ReturnsAssetsOrderedById()
    {
        await using (var ctx = NewContext())
        {
            var repo = new SqliteAssetRepository(ctx);
            await repo.UpsertAsync(MakeAsset(id: "A-3"));
            await repo.UpsertAsync(MakeAsset(id: "A-1"));
            await repo.UpsertAsync(MakeAsset(id: "A-2"));
        }

        await using (var ctx = NewContext())
        {
            var all = await new SqliteAssetRepository(ctx).GetAllAsync();
            Assert.Equal(new[] { "A-1", "A-2", "A-3" }, all.Select(a => a.Id).ToArray());
        }
    }

    public void Dispose() => _connection.Dispose();
}

namespace AssetTrack.App.Hosting;

using AssetTrack.App.Commands;
using AssetTrack.Core.Abstractions;
using AssetTrack.Core.Sync;
using AssetTrack.Data;
using AssetTrack.Data.Audit;
using AssetTrack.Data.Repositories;
using AssetTrack.Sync;
using AssetTrack.Sync.Transport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

/// <summary>Wires the dependency-injection container for the application.</summary>
public static class ServiceBootstrap
{
    public static ServiceProvider Build(string databasePath, string exchangePath)
    {
        var services = new ServiceCollection();

        services.AddDbContext<AssetDbContext>(options =>
            options.UseSqlite($"Data Source={databasePath}"));

        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<IAssetRepository, SqliteAssetRepository>();
        services.AddScoped<IAuditLog, SqliteAuditLog>();
        services.AddScoped<ConflictResolver>();
        services.AddScoped<SyncReconciler>();
        services.AddScoped<ISyncTransport>(_ => new FileSyncTransport(exchangePath));
        services.AddScoped<SyncEngine>();

        services.AddScoped<AddAssetCommand>();
        services.AddScoped<ListAssetsCommand>();
        services.AddScoped<SyncCommand>();

        return services.BuildServiceProvider();
    }
}

/// <summary>The production clock, returning real UTC time.</summary>
public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

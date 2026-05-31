using AssetTrack.App.Commands;
using AssetTrack.App.Hosting;
using AssetTrack.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

const string defaultDb = "assettrack.db";
const string defaultExchange = "data/exchange.json";

if (args.Length == 0)
{
    PrintUsage();
    return 1;
}

await using var provider = ServiceBootstrap.Build(defaultDb, defaultExchange);

// Offline-first: create the schema locally with no server required.
using (var initScope = provider.CreateScope())
{
    var db = initScope.ServiceProvider.GetRequiredService<AssetDbContext>();
    await db.Database.EnsureCreatedAsync();
}

using var scope = provider.CreateScope();
var sp = scope.ServiceProvider;

switch (args[0])
{
    case "add":
        if (args.Length < 3)
        {
            Console.Error.WriteLine("usage: add <id> <name> [location] [category]");
            return 1;
        }

        var add = sp.GetRequiredService<AddAssetCommand>();
        await add.ExecuteAsync(
            id: args[1],
            name: args[2],
            location: args.Length > 3 ? args[3] : null,
            category: args.Length > 4 ? args[4] : null);
        return 0;

    case "list":
        await sp.GetRequiredService<ListAssetsCommand>().ExecuteAsync();
        return 0;

    case "sync":
        await sp.GetRequiredService<SyncCommand>().ExecuteAsync();
        return 0;

    default:
        Console.Error.WriteLine($"unknown command: {args[0]}");
        PrintUsage();
        return 1;
}

static void PrintUsage()
{
    Console.WriteLine("assettrack — offline-first asset tracker");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  add <id> <name> [location] [category]   Add or update an asset");
    Console.WriteLine("  list                                    List all assets");
    Console.WriteLine("  sync                                    Reconcile with the exchange file");
}

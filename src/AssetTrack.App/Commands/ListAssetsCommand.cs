namespace AssetTrack.App.Commands;

using AssetTrack.Core.Abstractions;

/// <summary>Lists all recorded assets in a compact table.</summary>
public sealed class ListAssetsCommand
{
    private readonly IAssetRepository _repository;

    public ListAssetsCommand(IAssetRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync()
    {
        var assets = await _repository.GetAllAsync();
        if (assets.Count == 0)
        {
            Console.WriteLine("No assets recorded.");
            return;
        }

        Console.WriteLine($"{"ID",-16}{"NAME",-24}{"LOCATION",-16}{"STATE",-12}VER");
        foreach (var a in assets)
        {
            Console.WriteLine($"{a.Id,-16}{a.Name,-24}{(a.Location ?? "-"),-16}{a.SyncState,-12}{a.Version}");
        }
    }
}

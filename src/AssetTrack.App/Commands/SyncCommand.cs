namespace AssetTrack.App.Commands;

using AssetTrack.Sync;

/// <summary>Runs a single reconciliation cycle and prints the summary.</summary>
public sealed class SyncCommand
{
    private readonly SyncEngine _engine;

    public SyncCommand(SyncEngine engine)
    {
        _engine = engine;
    }

    public async Task ExecuteAsync()
    {
        var summary = await _engine.SyncAsync();
        Console.WriteLine(summary.Message);
    }
}

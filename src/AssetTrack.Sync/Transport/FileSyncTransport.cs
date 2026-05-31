namespace AssetTrack.Sync.Transport;

using System.Text.Json;
using AssetTrack.Core.Models;

/// <summary>
/// A file-based transport that reads and writes a JSON exchange file. Suitable
/// for offline reconciliation via shared folders or removable media, with no
/// data leaving the local boundary unless the file itself is moved.
/// </summary>
public sealed class FileSyncTransport : ISyncTransport
{
    private readonly string _exchangePath;

    public FileSyncTransport(string exchangePath)
    {
        _exchangePath = exchangePath;
    }

    public Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(_exchangePath);
        var available = string.IsNullOrEmpty(directory) || Directory.Exists(directory);
        return Task.FromResult(available);
    }

    public async Task<IReadOnlyList<Asset>> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_exchangePath))
        {
            return Array.Empty<Asset>();
        }

        await using var stream = File.OpenRead(_exchangePath);
        var assets = await JsonSerializer.DeserializeAsync<List<Asset>>(stream, cancellationToken: cancellationToken);
        return assets ?? new List<Asset>();
    }

    public async Task SendAsync(IReadOnlyList<Asset> assets, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(_exchangePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_exchangePath);
        await JsonSerializer.SerializeAsync(stream, assets, cancellationToken: cancellationToken);
    }
}

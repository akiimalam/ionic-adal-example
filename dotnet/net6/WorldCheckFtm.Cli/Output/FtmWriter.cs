using System.Text.Json;
using WorldCheckFtm.Cli.Domain;

namespace WorldCheckFtm.Cli.Output;

public static class FtmWriter
{
    public static async Task WriteNdjsonAsync(string path, IEnumerable<FtmEntity> entities, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        await using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        await using var writer = new StreamWriter(stream);

        foreach (var entity in entities)
        {
            var json = JsonSerializer.Serialize(entity);
            cancellationToken.ThrowIfCancellationRequested();
            await writer.WriteLineAsync(json);
        }
    }
}

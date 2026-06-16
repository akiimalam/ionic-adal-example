using System.Text.Json;
using WorldCheckFtm.Cli.Domain;

namespace WorldCheckFtm.Cli.Output;

public static class ReportWriter
{
    public static async Task WriteAsync(string path, ChangeReport report, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);

        var payload = new
        {
            Added = report.AddedIds.Count,
            Changed = report.ChangedIds.Count,
            Unchanged = report.UnchangedIds.Count,
            report.AddedIds,
            report.ChangedIds,
            report.UnchangedIds
        };

        await using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        await JsonSerializer.SerializeAsync(stream, payload, cancellationToken: cancellationToken);
    }
}

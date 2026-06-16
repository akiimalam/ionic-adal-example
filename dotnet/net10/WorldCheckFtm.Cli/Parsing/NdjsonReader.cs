using System.Text.Json;
using WorldCheckFtm.Cli.Domain;

namespace WorldCheckFtm.Cli.Parsing;

public static class NdjsonReader
{
    public static IEnumerable<FtmEntity> ReadFtmEntities(string path)
    {
        foreach (var line in File.ReadLines(path))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var entity = JsonSerializer.Deserialize<FtmEntity>(line);
            if (entity is not null)
            {
                yield return entity;
            }
        }
    }
}

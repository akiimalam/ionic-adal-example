using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WorldCheckFtm.Cli.Domain;

namespace WorldCheckFtm.Cli.Services;

public sealed class DiffService
{
    public ChangeReport Compare(IReadOnlyCollection<FtmEntity> current, IReadOnlyCollection<FtmEntity> previous)
    {
        var report = new ChangeReport();
        var previousHashes = BuildPreviousHashMap(previous);

        foreach (var entity in current)
        {
            var currentHash = ComputeSignature(entity);
            if (!previousHashes.TryGetValue(entity.Id, out var priorHash))
            {
                report.AddedIds.Add(entity.Id);
            }
            else if (!string.Equals(currentHash, priorHash, StringComparison.Ordinal))
            {
                report.ChangedIds.Add(entity.Id);
            }
            else
            {
                report.UnchangedIds.Add(entity.Id);
            }
        }

        return report;
    }

    private static Dictionary<string, string> BuildPreviousHashMap(IEnumerable<FtmEntity> entities)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entity in entities)
        {
            map[entity.Id] = ComputeSignature(entity);
        }

        return map;
    }

    private static string ComputeSignature(FtmEntity entity)
    {
        var canonicalProperties = entity.Properties
            .OrderBy(p => p.Key, StringComparer.Ordinal)
            .ToDictionary(
                p => p.Key,
                p => p.Value.OrderBy(v => v, StringComparer.Ordinal).ToArray(),
                StringComparer.Ordinal);

        var canonical = JsonSerializer.Serialize(new { entity.Schema, canonicalProperties, entity.Datasets });
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(canonical));
        return Convert.ToHexString(hash);
    }
}

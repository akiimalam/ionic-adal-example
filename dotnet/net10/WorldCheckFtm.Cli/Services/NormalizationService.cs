using System.Security.Cryptography;
using System.Text;
using WorldCheckFtm.Cli.Domain;

namespace WorldCheckFtm.Cli.Services;

public sealed class NormalizationService
{
    private readonly LookupCatalog _catalog;
    private readonly string _datasetName;

    public NormalizationService(LookupCatalog catalog, string datasetName)
    {
        _catalog = catalog;
        _datasetName = datasetName;
    }

    public IReadOnlyList<FtmEntity> ConvertToFtmEntities(IEnumerable<SourceProfileRecord> records)
    {
        var result = new List<FtmEntity>();
        foreach (var record in records)
        {
            var schema = ResolveSchema(record.Type);
            var subjectId = BuildId("subject", record.SourceId, schema, record.Name);
            var sanctionId = BuildId("sanction", record.SourceId, "Sanction", record.Name);

            var subject = new FtmEntity
            {
                Id = subjectId,
                Schema = schema,
                Datasets = new List<string> { _datasetName },
                Referents = new List<string> { record.SourceId },
                Properties = BuildSubjectProperties(record)
            };

            var sanction = new FtmEntity
            {
                Id = sanctionId,
                Schema = "Sanction",
                Datasets = new List<string> { _datasetName },
                Referents = new List<string> { record.SourceId },
                Properties = BuildSanctionProperties(record, subjectId)
            };

            result.Add(subject);
            result.Add(sanction);
        }

        return result;
    }

    private Dictionary<string, List<string>> BuildSubjectProperties(SourceProfileRecord record)
    {
        var properties = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = Single(record.Name),
            ["alias"] = record.Aliases,
            ["country"] = record.Countries,
            ["nationality"] = record.Nationalities,
            ["idNumber"] = ExpandIdTypes(record.IdNumbers)
        };

        AddIfPresent(properties, "birthDate", record.BirthDate);
        AddIfPresent(properties, "birthPlace", record.BirthPlace);

        return Cleanup(properties);
    }

    private Dictionary<string, List<string>> BuildSanctionProperties(SourceProfileRecord record, string subjectId)
    {
        var properties = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["entity"] = Single(subjectId),
            ["authority"] = Single(ExpandCode(_catalog.SourceCodeToLabel, record.Authority)),
            ["program"] = Single(record.Program),
            ["reason"] = Single(record.Reason)
        };

        AddIfPresent(properties, "listingDate", record.ListingDate);
        return Cleanup(properties);
    }

    private static string ResolveSchema(string type) =>
        type.Trim().ToLowerInvariant() switch
        {
            "person" or "individual" => "Person",
            "company" or "entity" => "Company",
            "organization" => "Organization",
            "vessel" => "Vessel",
            _ => "LegalEntity"
        };

    private List<string> ExpandIdTypes(IEnumerable<string> values)
    {
        return values
            .Select(value => ExpandCode(_catalog.IdTypeCodeToLabel, value))
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string ExpandCode(IReadOnlyDictionary<string, string> map, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return map.TryGetValue(value, out var expanded) ? expanded : value;
    }

    private static List<string> Single(string? value) =>
        string.IsNullOrWhiteSpace(value) ? new List<string>() : new List<string> { value.Trim() };

    private static void AddIfPresent(IDictionary<string, List<string>> properties, string key, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            properties[key] = new List<string> { value.Trim() };
        }
    }

    private static Dictionary<string, List<string>> Cleanup(Dictionary<string, List<string>> properties)
    {
        return properties
            .Where(pair => pair.Value is { Count: > 0 })
            .ToDictionary(
                pair => pair.Key,
                pair => pair.Value.Where(v => !string.IsNullOrWhiteSpace(v)).Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
                StringComparer.OrdinalIgnoreCase);
    }

    private static string BuildId(string prefix, string sourceId, string schema, string name)
    {
        var input = $"{prefix}|{sourceId}|{schema}|{name}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return $"wc-{prefix}-{Convert.ToHexString(hash)[..16].ToLowerInvariant()}";
    }
}

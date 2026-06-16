using WorldCheckFtm.Cli.Domain;
using WorldCheckFtm.Cli.Parsing;

namespace WorldCheckFtm.Cli.Services;

public static class ProfileImportService
{
    public static IEnumerable<SourceProfileRecord> ReadProfiles(string csvPath)
    {
        foreach (var row in CsvRowReader.ReadRows(csvPath))
        {
            yield return new SourceProfileRecord
            {
                SourceId = GetValue(row, "profile_id", "id", "record_id") ?? Guid.NewGuid().ToString("N"),
                Type = GetValue(row, "profile_type", "entity_type", "type") ?? "Person",
                Name = GetValue(row, "name", "full_name") ?? string.Empty,
                Aliases = SplitList(GetValue(row, "aliases", "aka")),
                Countries = SplitList(GetValue(row, "countries", "country")),
                Nationalities = SplitList(GetValue(row, "nationality", "nationalities")),
                IdNumbers = SplitList(GetValue(row, "id_numbers", "document_numbers")),
                BirthDate = GetValue(row, "birth_date", "dob"),
                BirthPlace = GetValue(row, "birth_place", "pob"),
                Program = GetValue(row, "program", "list_name"),
                Authority = GetValue(row, "authority", "source_name"),
                ListingDate = GetValue(row, "listing_date", "listed_on"),
                Reason = GetValue(row, "reason", "comments")
            };
        }
    }

    private static string? GetValue(IReadOnlyDictionary<string, string> row, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (row.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            foreach (var pair in row)
            {
                if (pair.Key.Replace(" ", "", StringComparison.Ordinal).Equals(key.Replace("_", "", StringComparison.Ordinal), StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrWhiteSpace(pair.Value))
                {
                    return pair.Value;
                }
            }
        }

        return null;
    }

    private static List<string> SplitList(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new List<string>();
        }

        return value
            .Split(new[] { ';', '|' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Trim())
            .Where(part => part.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}

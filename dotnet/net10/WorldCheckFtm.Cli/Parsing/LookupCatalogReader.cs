using WorldCheckFtm.Cli.Domain;

namespace WorldCheckFtm.Cli.Parsing;

public static class LookupCatalogReader
{
    public static LookupCatalog Build(string? keywordsPath, string? sicPath, string? idTypesPath)
    {
        return new LookupCatalog
        {
            SourceCodeToLabel = ReadLookup(keywordsPath, "code", "label"),
            SicCodeToLabel = ReadLookup(sicPath, "sic", "description"),
            IdTypeCodeToLabel = ReadLookup(idTypesPath, "id_type", "description")
        };
    }

    private static IReadOnlyDictionary<string, string> ReadLookup(string? path, string keyColumn, string valueColumn)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in CsvRowReader.ReadRows(path))
        {
            if (!TryGetValue(row, keyColumn, out var key) || !TryGetValue(row, valueColumn, out var value))
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                map[key] = value;
            }
        }

        return map;
    }

    private static bool TryGetValue(IReadOnlyDictionary<string, string> row, string key, out string value)
    {
        if (row.TryGetValue(key, out value!))
        {
            return true;
        }

        foreach (var pair in row)
        {
            if (pair.Key.Replace(" ", "", StringComparison.Ordinal).Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                value = pair.Value;
                return true;
            }
        }

        value = string.Empty;
        return false;
    }
}

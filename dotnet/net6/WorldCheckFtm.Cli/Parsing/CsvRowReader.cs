namespace WorldCheckFtm.Cli.Parsing;

public static class CsvRowReader
{
    public static IEnumerable<Dictionary<string, string>> ReadRows(string path)
    {
        using var reader = new StreamReader(path);
        var headerLine = reader.ReadLine();
        if (string.IsNullOrWhiteSpace(headerLine))
        {
            yield break;
        }

        var headers = ParseCsvLine(headerLine).ToArray();
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var values = ParseCsvLine(line).ToArray();
            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < headers.Length; i++)
            {
                var value = i < values.Length ? values[i] : string.Empty;
                row[headers[i]] = value.Trim();
            }

            yield return row;
        }
    }

    private static IEnumerable<string> ParseCsvLine(string line)
    {
        var current = new System.Text.StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];
            if (ch == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }

                continue;
            }

            if (ch == ',' && !inQuotes)
            {
                yield return current.ToString();
                current.Clear();
                continue;
            }

            current.Append(ch);
        }

        yield return current.ToString();
    }
}

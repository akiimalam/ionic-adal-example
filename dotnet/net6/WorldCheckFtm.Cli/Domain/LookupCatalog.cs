namespace WorldCheckFtm.Cli.Domain;

public sealed class LookupCatalog
{
    public IReadOnlyDictionary<string, string> SourceCodeToLabel { get; init; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyDictionary<string, string> SicCodeToLabel { get; init; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyDictionary<string, string> IdTypeCodeToLabel { get; init; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}

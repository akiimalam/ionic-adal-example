namespace WorldCheckFtm.Cli.Domain;

public sealed class SourceProfileRecord
{
    public string SourceId { get; init; } = string.Empty;
    public string Type { get; init; } = "Person";
    public string Name { get; init; } = string.Empty;
    public List<string> Aliases { get; init; } = new();
    public List<string> Countries { get; init; } = new();
    public List<string> Nationalities { get; init; } = new();
    public List<string> IdNumbers { get; init; } = new();
    public string? BirthDate { get; init; }
    public string? BirthPlace { get; init; }
    public string? Program { get; init; }
    public string? Authority { get; init; }
    public string? ListingDate { get; init; }
    public string? Reason { get; init; }
}

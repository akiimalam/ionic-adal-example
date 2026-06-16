namespace WorldCheckFtm.Cli.Domain;

public sealed class ChangeReport
{
    public List<string> AddedIds { get; } = new();
    public List<string> ChangedIds { get; } = new();
    public List<string> UnchangedIds { get; } = new();
}

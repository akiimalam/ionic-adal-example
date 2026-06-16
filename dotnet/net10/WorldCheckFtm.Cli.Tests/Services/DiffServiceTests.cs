using WorldCheckFtm.Cli.Domain;
using WorldCheckFtm.Cli.Services;

namespace WorldCheckFtm.Cli.Tests.Services;

public sealed class DiffServiceTests
{
    [Fact]
    public void Compare_DetectsAddedAndChangedEntities()
    {
        var previous = new[]
        {
            BuildEntity("A", "name", "Alpha"),
            BuildEntity("B", "name", "Beta")
        };

        var current = new[]
        {
            BuildEntity("A", "name", "Alpha 2"),
            BuildEntity("B", "name", "Beta"),
            BuildEntity("C", "name", "Gamma")
        };

        var report = new DiffService().Compare(current, previous);

        Assert.Contains("A", report.ChangedIds);
        Assert.Contains("C", report.AddedIds);
        Assert.Contains("B", report.UnchangedIds);
    }

    private static FtmEntity BuildEntity(string id, string property, string value) =>
        new()
        {
            Id = id,
            Schema = "Person",
            Datasets = new List<string> { "worldcheck_custom" },
            Properties = new Dictionary<string, List<string>>
            {
                [property] = new List<string> { value }
            }
        };
}

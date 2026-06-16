using WorldCheckFtm.Cli.Parsing;

namespace WorldCheckFtm.Cli.Tests.Parsing;

public sealed class CsvRowReaderTests
{
    [Fact]
    public void ReadRows_SupportsQuotedCommas()
    {
        var file = Path.GetTempFileName();
        try
        {
            File.WriteAllText(file, "id,name,aliases\n1,\"Doe, Jane\",\"JD;Janie\"\n");
            var row = CsvRowReader.ReadRows(file).Single();

            Assert.Equal("1", row["id"]);
            Assert.Equal("Doe, Jane", row["name"]);
            Assert.Equal("JD;Janie", row["aliases"]);
        }
        finally
        {
            File.Delete(file);
        }
    }
}

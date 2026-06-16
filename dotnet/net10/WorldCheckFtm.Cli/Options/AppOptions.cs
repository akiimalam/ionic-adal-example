namespace WorldCheckFtm.Cli.Options;

public sealed class AppOptions
{
    public string ProfilesCsvPath { get; set; } = string.Empty;
    public string OutputFtmPath { get; set; } = "worldcheck-output.ftm.json";
    public string ReportPath { get; set; } = "worldcheck-report.json";
    public string DatasetName { get; set; } = "worldcheck_custom";
    public string? PreviousSnapshotPath { get; set; }
    public string? KeywordsPath { get; set; }
    public string? SicPath { get; set; }
    public string? IdTypesPath { get; set; }
}

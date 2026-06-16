using WorldCheckFtm.Cli.Options;
using WorldCheckFtm.Cli.Output;
using WorldCheckFtm.Cli.Services;

var options = ParseArgs(args);
if (options is null)
{
    PrintUsage();
    return 1;
}

var workflow = new WorkflowService();
var (entities, report) = workflow.Execute(options);

await FtmWriter.WriteNdjsonAsync(options.OutputFtmPath, entities);
await ReportWriter.WriteAsync(options.ReportPath, report);

Console.WriteLine($"Converted entities: {entities.Count}");
Console.WriteLine($"Added: {report.AddedIds.Count}, Changed: {report.ChangedIds.Count}, Unchanged: {report.UnchangedIds.Count}");
Console.WriteLine($"FTM NDJSON: {Path.GetFullPath(options.OutputFtmPath)}");
Console.WriteLine($"Change report: {Path.GetFullPath(options.ReportPath)}");
return 0;

static AppOptions? ParseArgs(IReadOnlyList<string> args)
{
    var options = new AppOptions();

    for (var i = 0; i < args.Count; i += 2)
    {
        if (i + 1 >= args.Count)
        {
            return null;
        }

        var key = args[i].Trim().ToLowerInvariant();
        var value = args[i + 1];

        switch (key)
        {
            case "--profiles-csv":
                options.ProfilesCsvPath = value;
                break;
            case "--output-ftm":
                options.OutputFtmPath = value;
                break;
            case "--report":
                options.ReportPath = value;
                break;
            case "--dataset":
                options.DatasetName = value;
                break;
            case "--previous-ftm":
                options.PreviousSnapshotPath = value;
                break;
            case "--keywords-csv":
                options.KeywordsPath = value;
                break;
            case "--sic-csv":
                options.SicPath = value;
                break;
            case "--id-types-csv":
                options.IdTypesPath = value;
                break;
            default:
                return null;
        }
    }

    return string.IsNullOrWhiteSpace(options.ProfilesCsvPath) ? null : options;
}

static void PrintUsage()
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run -- --profiles-csv <path> [--output-ftm <path>] [--report <path>] [--dataset <name>]");
    Console.WriteLine("               [--previous-ftm <path>] [--keywords-csv <path>] [--sic-csv <path>] [--id-types-csv <path>]");
}

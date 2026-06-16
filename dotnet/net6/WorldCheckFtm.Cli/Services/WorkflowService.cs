using WorldCheckFtm.Cli.Domain;
using WorldCheckFtm.Cli.Options;
using WorldCheckFtm.Cli.Parsing;

namespace WorldCheckFtm.Cli.Services;

public sealed class WorkflowService
{
    public (IReadOnlyList<FtmEntity> entities, ChangeReport report) Execute(AppOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ProfilesCsvPath) || !File.Exists(options.ProfilesCsvPath))
        {
            throw new FileNotFoundException("Input CSV path was not found", options.ProfilesCsvPath);
        }

        var catalog = LookupCatalogReader.Build(options.KeywordsPath, options.SicPath, options.IdTypesPath);
        var records = ProfileImportService.ReadProfiles(options.ProfilesCsvPath);
        var normalizer = new NormalizationService(catalog, options.DatasetName);
        var current = normalizer.ConvertToFtmEntities(records).ToList();

        var previous = new List<FtmEntity>();
        if (!string.IsNullOrWhiteSpace(options.PreviousSnapshotPath) && File.Exists(options.PreviousSnapshotPath))
        {
            previous = NdjsonReader.ReadFtmEntities(options.PreviousSnapshotPath).ToList();
        }

        var diffService = new DiffService();
        var report = diffService.Compare(current, previous);
        return (current, report);
    }
}

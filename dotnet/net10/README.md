# WorldCheck FtM Converter (.NET 10)

## Build

```bash
dotnet build /home/runner/work/ionic-adal-example/ionic-adal-example/akiimalam/ionic-adal-example/dotnet/net10/WorldCheckFtm.Net10.slnx
```

## Test

```bash
dotnet test /home/runner/work/ionic-adal-example/ionic-adal-example/akiimalam/ionic-adal-example/dotnet/net10/WorldCheckFtm.Net10.slnx
```

## Run

```bash
dotnet run --project /home/runner/work/ionic-adal-example/ionic-adal-example/akiimalam/ionic-adal-example/dotnet/net10/WorldCheckFtm.Cli/WorldCheckFtm.Cli.csproj -- \
  --profiles-csv /absolute/path/Sample\ files_advanced.csv \
  --keywords-csv /absolute/path/worldcheck_keywords.csv \
  --sic-csv /absolute/path/worldcheck_sic.csv \
  --id-types-csv /absolute/path/worldcheck_id_types.csv \
  --previous-ftm /absolute/path/entities.ftm.json \
  --output-ftm /absolute/path/worldcheck-normalized.ftm.json \
  --report /absolute/path/worldcheck-diff-report.json
```

# WorldCheck FtM Converter (.NET 6)

## Build

```bash
dotnet build dotnet/net6/WorldCheckFtm.Net6.slnx
```

## Test

```bash
dotnet test dotnet/net6/WorldCheckFtm.Net6.slnx
```

## Run

```bash
dotnet run --project /home/runner/work/ionic-adal-example/ionic-adal-example/akiimalam/ionic-adal-example/dotnet/net6/WorldCheckFtm.Cli/WorldCheckFtm.Cli.csproj -- \
  --profiles-csv /absolute/path/Sample\ files_advanced.csv \
  --keywords-csv /absolute/path/worldcheck_keywords.csv \
  --sic-csv /absolute/path/worldcheck_sic.csv \
  --id-types-csv /absolute/path/worldcheck_id_types.csv \
  --previous-ftm /absolute/path/entities.ftm.json \
  --output-ftm /absolute/path/worldcheck-normalized.ftm.json \
  --report /absolute/path/worldcheck-diff-report.json
```

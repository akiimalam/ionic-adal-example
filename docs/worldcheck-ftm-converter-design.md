# World-Check to FtM Converter Design

## 1) Problem statement and goals

This repository currently hosts an Ionic sample app. The new requirement is to add a maintainable utility design and implementation to convert mixed World-Check style source data into FollowTheMoney (FtM) NDJSON so it can be ingested by OpenSanctions/Yente workflows.

Goals:
- Normalize mixed source inputs into FtM-compatible entities.
- Preserve source referents and generate stable IDs.
- Produce a deterministic change report that highlights newly added and changed records.
- Provide two structurally similar implementations: one on .NET 10 and one on .NET 6.

## 2) Supported input formats and assumptions

Supported inputs in this design:
- Profile CSV exports (example: advanced profile datasets).
- NDJSON FtM baselines (`entities.ftm.json`) used as prior snapshots.
- Lookup CSV files for keyword/source labels, SIC labels, and ID type labels.
- Reference PDF guides are treated as metadata references (assumptions and mappings documented here; PDFs are not parsed directly by this utility).

Assumptions:
- CSV headers vary by feed; fallback header aliases are used (`profile_id`/`id`/`record_id`, etc.).
- Multi-valued CSV fields use `;` or `|` separators.
- Source IDs are expected when possible; if missing, a generated fallback ID is used.

## 3) Target output / normalized domain model

Output is NDJSON FtM-like entities:
- Subject entity (`Person`, `Company`, `Organization`, `Vessel`, or `LegalEntity`).
- Linked `Sanction` entity with `entity` property referencing the subject ID.

Core normalized fields:
- `id`, `schema`, `datasets`, `referents`, `properties`.
- Typical properties: `name`, `alias`, `country`, `nationality`, `idNumber`, `birthDate`, `birthPlace`, `authority`, `program`, `listingDate`, `reason`.

IDs are deterministic hashes of source ID + schema + name, enabling stable comparison across runs.

## 4) Detecting what is new in the data

A diff step compares current output against a prior FtM NDJSON snapshot:
1. Canonicalize each entity (`schema`, sorted properties, sorted property values, datasets).
2. Hash canonical payload.
3. Compare by `id`:
   - Missing in previous => **Added**
   - Present but hash changed => **Changed**
   - Present and hash equal => **Unchanged**

The report is emitted as JSON with counts and IDs.

## 5) Architecture and component boundaries

Each runtime has the same layered structure:
- `Domain/`: data contracts (`SourceProfileRecord`, `FtmEntity`, `LookupCatalog`, `ChangeReport`).
- `Parsing/`: CSV row parser, NDJSON reader, lookup loaders.
- `Services/`: profile import mapping, normalization, diff logic, workflow orchestration.
- `Output/`: NDJSON writer and report writer.
- `Options/`: CLI option model.
- `Program.cs`: argument parsing and app entrypoint.

This keeps parsing, transformation, and reporting isolated for easier extension and testing.

## 6) Parsing strategy

### CSV
- Custom parser handles quoted fields and escaped quotes.
- Header aliasing tolerates column name differences.
- Multi-value splitting normalizes aliases, countries, nationalities, and IDs.

### NDJSON
- Line-by-line deserialization for low memory pressure and stream-friendly processing.

### Reference metadata
- Keyword/source, SIC, and ID type lookup files are loaded into dictionaries to expand source codes into human-readable values during normalization.

## 7) Performance considerations

- Streaming read patterns (`File.ReadLines`, line-by-line CSV processing).
- O(n) diff map keyed by entity ID.
- Deterministic hashing avoids expensive deep object diffing across snapshots.
- Output writers stream to disk and avoid holding serialized payloads in memory.

Future optimization points:
- Parallel parsing for very large profile files.
- Batched writer buffers.
- Incremental checkpointing by source partition/date.

## 8) Why C# for production throughput (vs Python)

C# benefits for high-throughput production ingestion:
- Strong static typing and compile-time checks reduce runtime data-shape errors.
- Faster CPU-bound transforms and hashing in long-running pipelines.
- Mature async/streaming APIs and robust deployment/runtime tooling.

Python tradeoffs:
- Usually faster prototyping and data science ecosystem integration.
- Potentially slower for sustained large-volume transforms unless optimized with extensions/vectorized tooling.

Recommended posture:
- Use Python for rapid exploration/prototyping.
- Use C# services when throughput, operational consistency, and typed contracts are primary concerns.

## 9) Project layout for .NET 10 and .NET 6

```text
/dotnet
  /net10
    WorldCheckFtm.Net10.slnx
    README.md
    /WorldCheckFtm.Cli
      Program.cs
      /Domain
      /Options
      /Parsing
      /Services
      /Output
    /WorldCheckFtm.Cli.Tests
      /Parsing
      /Services

  /net6
    WorldCheckFtm.Net6.slnx
    README.md
    /WorldCheckFtm.Cli
      Program.cs
      /Domain
      /Options
      /Parsing
      /Services
      /Output
    /WorldCheckFtm.Cli.Tests
      /Parsing
      /Services
```

## 10) Testing strategy and extension points

Current tests cover:
- CSV parser handling quoted values with commas.
- Diff service classification of added/changed/unchanged entities.

Extension points:
- Add schema-specific mappers (e.g., aircraft or address-rich profiles).
- Add robust validation against FtM schema metadata.
- Add source-specific parsers for additional World-Check exports.
- Add PDF metadata extraction module when source PDFs need direct machine parsing.

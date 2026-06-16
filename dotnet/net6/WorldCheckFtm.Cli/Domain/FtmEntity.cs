using System.Text.Json.Serialization;

namespace WorldCheckFtm.Cli.Domain;

public sealed class FtmEntity
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("schema")]
    public string Schema { get; init; } = string.Empty;

    [JsonPropertyName("properties")]
    public Dictionary<string, List<string>> Properties { get; init; } = new(StringComparer.OrdinalIgnoreCase);

    [JsonPropertyName("datasets")]
    public List<string> Datasets { get; init; } = new();

    [JsonPropertyName("referents")]
    public List<string> Referents { get; init; } = new();
}

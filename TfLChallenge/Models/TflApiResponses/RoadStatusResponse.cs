using System.Text.Json.Serialization;

namespace TfLChallenge.Models.TflApiResponses;

public class RoadStatusResponse
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; }

    [JsonPropertyName("statusSeverity")]
    public string StatusSeverity { get; set; }

    [JsonPropertyName("statusSeverityDescription")]
    public string StatusSeverityDescription { get; set; }

    [JsonPropertyName("bounds")]
    public string Bounds { get; set; }

    [JsonPropertyName("envelope")]
    public string Envelope { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}
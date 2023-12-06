using System.Text.Json.Serialization;

namespace Imaging.Infrastructure.ExternalApi.Bing.BingModels;

/// <summary>
/// Defines the pivot segment.
/// </summary>
public class PivotSuggestions
{
    /// <summary>
    /// Gets or sets the segment from the original query to pivot on.
    /// </summary>
    [JsonPropertyName("pivot")]
    public string? Pivot { get; set; }

    /// <summary>
    /// Gets or sets a list of suggested queries for the pivot.
    /// </summary>
    [JsonPropertyName("suggestions")]
    public IList<Query>? Suggestions { get; set; }
}

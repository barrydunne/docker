using System.Text.Json.Serialization;

namespace Imaging.Infrastructure.ExternalApi.Bing.BingModels;

/// <summary>
/// Defines an item.
/// </summary>
// [JsonPath("Properties/Item")]
public class PropertiesItem
{
    /// <summary>
    /// Gets or sets text representation of an item.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}

using System.Text.Json.Serialization;

namespace Imaging.Infrastructure.ExternalApi.Bing.BingModels;

/// <summary>
/// Defines the identity of a resource.
/// </summary>
public class Identifiable
{
    /// <summary>
    /// Gets or sets a String identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }
}

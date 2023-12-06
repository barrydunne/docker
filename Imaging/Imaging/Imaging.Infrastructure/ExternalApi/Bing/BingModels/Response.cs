using System.Text.Json.Serialization;

namespace Imaging.Infrastructure.ExternalApi.Bing.BingModels;

/// <summary>
/// Defines a response. All schemas that could be returned at the root of a response should inherit from this.
/// </summary>
public class Response : Identifiable
{
    /// <summary>
    /// Gets or sets the URL that returns this resource.
    /// </summary>
    [JsonPropertyName("readLink")]
    public string? ReadLink { get; set; }

    /// <summary>
    /// Gets or sets the URL To Bing's search result for this item.
    /// </summary>
    [JsonPropertyName("webSearchUrl")]
    public string? WebSearchUrl { get; set; }

    /// <summary>
    /// Gets or sets or sets a list of errors that describe the reasons why the request failed.
    /// </summary>
    [JsonPropertyName("errors")]
    public IList<Error>? Errors { get; set; }
}

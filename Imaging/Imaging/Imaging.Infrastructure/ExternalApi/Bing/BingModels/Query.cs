using System.Text.Json.Serialization;

namespace Imaging.Infrastructure.ExternalApi.Bing.BingModels;

/// <summary>Defines a search query.</summary>
public class Query
{
    /// <summary>
    /// Gets or sets the query string. Use this string as the query term in a new search request.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the display version of the query term.
    /// This version of the query term may contain special characters that highlight the search term found in the query string.
    /// The string contains the highlighting characters only if the query enabled hit highlighting.
    /// </summary>
    [JsonPropertyName("displayText")]
    public string? DisplayText { get; set; }

    /// <summary>
    /// Gets or sets the URL that takes the user to the Bing search results page for the query.Only related search results include this field.
    /// </summary>
    [JsonPropertyName("webSearchUrl")]
    public string? WebSearchUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL that you use to get the results of the related search.
    /// Before using the URL, you must append query parameters as appropriate and include the Ocp-Apim-Subscription-Key header.
    /// Use this URL if you're displaying the results in your own user interface. Otherwise, use the webSearchUrl URL.
    /// </summary>
    [JsonPropertyName("searchLink")]
    public string? SearchLink { get; set; }

    /// <summary>
    /// Gets or sets the URL to a thumbnail of a related image.
    /// </summary>
    [JsonPropertyName("thumbnail")]
    public ImageObject? Thumbnail { get; set; }
}

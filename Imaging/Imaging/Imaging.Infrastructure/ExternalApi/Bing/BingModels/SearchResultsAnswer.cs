using System.Text.Json.Serialization;

namespace Imaging.Infrastructure.ExternalApi.Bing.BingModels;

/// <summary>Defines a search result answer.</summary>
public class SearchResultsAnswer : Response
{
    /// <summary>
    /// Gets or sets the estimated number of webpages that are relevant to the query.
    /// Use this number along with the count and offset query parameters to page the results.
    /// </summary>
    [JsonPropertyName("totalEstimatedMatches")]
    public long? TotalEstimatedMatches { get; set; }
}

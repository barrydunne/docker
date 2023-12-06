using System.Text.Json.Serialization;

namespace Imaging.Infrastructure.ExternalApi.Bing.BingModels;

/// <summary>
/// Defines a rating.
/// </summary>
public class Rating : PropertiesItem
{
    /// <summary>
    /// Gets or sets the mean (average) rating. The possible values are 1.0 through 5.0.
    /// </summary>
    [JsonPropertyName("ratingValue")]
    public double? RatingValue { get; set; }

    /// <summary>
    /// Gets or sets the highest rated review. The possible values are 1.0 through 5.0.
    /// </summary>
    [JsonPropertyName("bestRating")]
    public double? BestRating { get; set; }
}

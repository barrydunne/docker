using System.Text.Json.Serialization;

namespace Imaging.Infrastructure.ExternalApi.Bing.BingModels;

/// <summary>
/// Defines a count of the number of websites where you can shop or perform other actions related to the image.
/// </summary>
public class ImagesImageMetadata
{
    /// <summary>
    /// Gets or sets the number of websites that offer goods of the products seen in the image.
    /// </summary>
    [JsonPropertyName("shoppingSourcesCount")]
    public int? ShoppingSourcesCount { get; set; }

    /// <summary>
    /// Gets or sets the number of websites that offer recipes of the food seen in the image.
    /// </summary>
    [JsonPropertyName("recipeSourcesCount")]
    public int? RecipeSourcesCount { get; set; }

    /// <summary>
    /// Gets or sets a summary of the online offers of products found in the image.
    /// For example, if the image is of a dress, the offer might identify the lowest price and the number of offers found.
    /// Only visually similar products insights include this field. The offer includes the following fields: Name, AggregateRating, OfferCount, and LowPrice.
    /// </summary>
    [JsonPropertyName("aggregateOffer")]
    public AggregateOffer? AggregateOffer { get; set; }
}

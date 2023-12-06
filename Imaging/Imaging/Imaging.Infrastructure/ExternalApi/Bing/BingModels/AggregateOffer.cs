using System.Text.Json.Serialization;

namespace Imaging.Infrastructure.ExternalApi.Bing.BingModels;

/// <summary>
/// Defines a list of offers from merchants that are related to the image.
/// </summary>
public class AggregateOffer : Offer
{
    /// <summary>
    /// Gets or sets a list of offers from merchants that have offerings related to the image.
    /// </summary>
    [JsonPropertyName("offers")]
    public IList<Offer>? Offers { get; set; }
}

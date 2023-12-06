using System.Text.Json.Serialization;

namespace Imaging.Infrastructure.ExternalApi.Bing.BingModels;

/// <summary>
/// Defines an image.
/// </summary>
public class ImageObject : MediaObject
{
    /// <summary>
    /// Gets or sets the URL to a thumbnail of the image.
    /// </summary>
    [JsonPropertyName("thumbnail")]
    public ImageObject? Thumbnail { get; set; }

    /// <summary>
    /// Gets or sets the token that you use in a subsequent call to the Image Search API to get additional information about the image.
    /// For information about using this token, see the insightsToken query parameter.
    /// </summary>
    [JsonPropertyName("imageInsightsToken")]
    public string? ImageInsightsToken { get; set; }

    /// <summary>
    /// Gets or sets a count of the number of websites where you can shop or perform other actions related to the image.
    /// For example, if the image is of an apple pie, this object includes a count of the number of websites where you can buy an apple pie.
    /// To indicate the number of offers in your UX, include badging such as a shopping cart icon that contains the count. When the user clicks on the icon, use imageInisghtsToken to get the list of websites.
    /// </summary>
    [JsonPropertyName("insightsMetadata")]
    public ImagesImageMetadata? InsightsMetadata { get; set; }

    /// <summary>
    /// Gets or sets unique Id for the image.
    /// </summary>
    [JsonPropertyName("imageId")]
    public string? ImageId { get; set; }

    /// <summary>
    /// Gets or sets a three-byte hexadecimal number that represents the color that dominates the image. Use the color as the temporary background in your client until the image is loaded.
    /// </summary>
    [JsonPropertyName("accentColor")]
    public string? AccentColor { get; set; }

    /// <summary>
    /// Gets or sets visual representation of the image. Used for getting more sizes.
    /// </summary>
    [JsonPropertyName("visualWords")]
    public string? VisualWords { get; set; }
}

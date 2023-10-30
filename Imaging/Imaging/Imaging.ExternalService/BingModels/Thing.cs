using System.Text.Json.Serialization;

namespace Imaging.ExternalService.BingModels
{
    /// <summary>
    /// Defines a thing.
    /// </summary>
    public class Thing : Response
    {
        /// <summary>
        /// Gets or sets the name of the thing represented by this object.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the URL to get more information about the thing represented by this object.
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        /// <summary>
        /// Gets or sets an image of the item.
        /// </summary>
        [JsonPropertyName("image")]
        public ImageObject? Image { get; set; }

        /// <summary>
        /// Gets or sets a short description of the item.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets an alias for the item.
        /// </summary>
        [JsonPropertyName("alternateName")]
        public string? AlternateName { get; set; }

        /// <summary>
        /// Gets or sets an ID that uniquely identifies this item.
        /// </summary>
        [JsonPropertyName("bingId")]
        public string? BingId { get; set; }
    }
}

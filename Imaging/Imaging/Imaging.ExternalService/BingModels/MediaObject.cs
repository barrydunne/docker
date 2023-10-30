using System.Text.Json.Serialization;

namespace Imaging.ExternalService.BingModels
{
    /// <summary>
    /// Defines a media object.
    /// </summary>
    public class MediaObject : CreativeWork
    {
        /// <summary>
        /// Gets or sets original URL to retrieve the source (file) for the media object (e.g the source URL for the image).
        /// </summary>
        [JsonPropertyName("contentUrl")]
        public string? ContentUrl { get; set; }

        /// <summary>
        /// Gets or sets URL of the page that hosts the media object.
        /// </summary>
        [JsonPropertyName("hostPageUrl")]
        public string? HostPageUrl { get; set; }

        /// <summary>
        /// Gets or sets the date the page that hosts the media object was discovered.
        /// </summary>
        [JsonPropertyName("hostPageDiscoveredDate")]
        public DateTime? HostPageDiscoveredDate { get; set; }

        /// <summary>
        /// Gets or sets size of the media object content (use format "value unit" e.g "1024 B").
        /// </summary>
        [JsonPropertyName("contentSize")]
        public string? ContentSize { get; set; }

        /// <summary>
        /// Gets or sets encoding format (e.g mp3, mp4, jpeg, etc).
        /// </summary>
        [JsonPropertyName("encodingFormat")]
        public string? EncodingFormat { get; set; }

        /// <summary>
        /// Gets or sets display URL of the page that hosts the media object.
        /// </summary>
        [JsonPropertyName("hostPageDisplayUrl")]
        public string? HostPageDisplayUrl { get; set; }

        /// <summary>
        /// Gets or sets the width of the source media object, in pixels.
        /// </summary>
        [JsonPropertyName("width")]
        public int? Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the source media object, in pixels.
        /// </summary>
        [JsonPropertyName("height")]
        public int? Height { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace Imaging.ExternalService.BingModels
{
    /// <summary>
    /// Defines the metrics that indicate how well an item was rated by others.
    /// </summary>
    public class AggregateRating : Rating
    {
        /// <summary>
        /// Gets or sets the number of times the recipe has been rated or reviewed.
        /// </summary>
        [JsonPropertyName("reviewCount")]
        public int? ReviewCount { get; set; }
    }
}

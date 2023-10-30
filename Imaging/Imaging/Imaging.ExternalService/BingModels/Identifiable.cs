using System.Text.Json.Serialization;

namespace Imaging.ExternalService.BingModels
{
    /// <summary>
    /// Defines the identity of a resource.
    /// </summary>
    public class Identifiable
    {
        /// <summary>
        /// Gets or sets a String identifier.
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }
}

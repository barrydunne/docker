using System.Text.Json.Serialization;

namespace Imaging.Infrastructure.ExternalApi.Bing.BingModels;

/// <summary>
/// The most generic kind of creative work, including books, movies, photographs, software programs, etc.
/// </summary>
public class CreativeWork : Thing
{
    /// <summary>
    /// Gets or sets the URL to a thumbnail of the item.
    /// </summary>
    [JsonPropertyName("thumbnailUrl")]
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Gets or sets the source of the creative work.
    /// </summary>
    [JsonPropertyName("provider")]
    public IList<Thing>? Provider { get; set; }

    /// <summary>
    /// Gets or sets the date on which the CreativeWork was published.
    /// </summary>
    [JsonPropertyName("datePublished")]
    public DateTime? DatePublished { get; set; }

    /// <summary>
    /// Gets or sets text content of this creative work.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets whether this work is family friendly.
    /// </summary>
    public bool? IsFamilyFriendly { get; set; }
}

using System.Text.Json.Serialization;

namespace Imaging.ExternalService.BingModels
{
    /// <summary>
    /// Defines an image answer.
    /// </summary>
    public partial class ImagesResponse : SearchResultsAnswer
    {
        /// <summary>
        /// Gets or sets used as part of deduping. Tells client the next offset that client should use in the next pagination request.
        /// </summary>
        [JsonPropertyName("nextOffset")]
        public int? NextOffset { get; set; }

        /// <summary>
        /// Gets or sets a list of image objects that are relevant to the query. If there are no results, the List is empty.
        /// </summary>
        [JsonPropertyName("value")]
        public IList<ImageObject>? Value { get; set; }

        /// <summary>
        /// Gets or sets a list of expanded queries that narrows the original query.
        /// For example, if the query was Microsoft Surface, the expanded queries might be: Microsoft Surface Pro 3, Microsoft Surface RT, Microsoft Surface Phone, and Microsoft Surface Hub.
        /// </summary>
        [JsonPropertyName("queryExpansions")]
        public IList<Query>? QueryExpansions { get; set; }

        /// <summary>
        /// Gets or sets a list of segments in the original query. For example, if the query was Red Flowers, Bing might segment the query into Red and Flowers.
        /// The Flowers pivot may contain query suggestions such as Red Peonies and Red Daisies, and the Red pivot may contain query suggestions such as Green Flowers and Yellow Flowers.
        /// </summary>
        [JsonPropertyName("pivotSuggestions")]
        public IList<PivotSuggestions>? PivotSuggestions { get; set; }

        /// <summary>
        /// Gets or sets a list of terms that are similar in meaning to the user's query term.
        /// </summary>
        [JsonPropertyName("similarTerms")]
        public IList<Query>? SimilarTerms { get; set; }
    }
}

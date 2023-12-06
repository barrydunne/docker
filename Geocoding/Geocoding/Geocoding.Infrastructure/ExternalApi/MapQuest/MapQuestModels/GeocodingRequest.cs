namespace Geocoding.Infrastructure.ExternalApi.MapQuest.MapQuestModels;

// These classes were generated from the API request

public class GeocodingRequest
{
    public string? Location { get; set; }
    public GeocodingRequestOptions? Options { get; set; } = new();
}

public class GeocodingRequestOptions
{
    public bool? ThumbMaps { get; set; } = false;
    public int? MaxResults { get; set; } = 1;
}

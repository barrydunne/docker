namespace Geocoding.Infrastructure.ExternalApi.MapQuest.MapQuestModels;

// These classes were generated from the API response

public class GeocodingResponse
{
    public GeocodingResponseInfo? Info { get; set; }
    public GeocodingResponseOptions? Options { get; set; }
    public IReadOnlyList<GeocodingResponseResult>? Results { get; set; }
}

public class GeocodingResponseInfo
{
    public int? StatusCode { get; set; }
    public GeocodingResponseCopyright? Copyright { get; set; }
    public string[]? Messages { get; set; }
}

public class GeocodingResponseCopyright
{
    public string? Text { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageAltText { get; set; }
}

public class GeocodingResponseOptions
{
    public int? MaxResults { get; set; }
    public bool? IgnoreLatLngInput { get; set; }
}

public class GeocodingResponseResult
{
    public GeocodingResponseProvidedLocation? ProvidedLocation { get; set; }
    public IReadOnlyList<GeocodingResponseLocation>? Locations { get; set; }
}

public class GeocodingResponseProvidedLocation
{
    public string? Street { get; set; }
}

public class GeocodingResponseLocation
{
    public string? Street { get; set; }
    public string? AdminArea1 { get; set; }
    public string? AdminArea1Type { get; set; }
    public string? AdminArea2 { get; set; }
    public string? AdminArea2Type { get; set; }
    public string? AdminArea3 { get; set; }
    public string? AdminArea3Type { get; set; }
    public string? AdminArea4 { get; set; }
    public string? AdminArea4Type { get; set; }
    public string? AdminArea5 { get; set; }
    public string? AdminArea5Type { get; set; }
    public string? AdminArea6 { get; set; }
    public string? AdminArea6Type { get; set; }
    public string? PostalCode { get; set; }
    public string? GeocodeQualityCode { get; set; }
    public string? GeocodeQuality { get; set; }
    public bool? DragPoint { get; set; }
    public string? SideOfStreet { get; set; }
    public string? LinkId { get; set; }
    public string? UnknownInput { get; set; }
    public string? Type { get; set; }
    public GeocodingResponseLatLng? LatLng { get; set; }
    public GeocodingResponseDisplayLatLng? DisplayLatLng { get; set; }
    public string? MapUrl { get; set; }
}

public class GeocodingResponseLatLng
{
    public double? Lat { get; set; }
    public double? Lng { get; set; }
}

public class GeocodingResponseDisplayLatLng
{
    public double? Lat { get; set; }
    public double? Lng { get; set; }
}

namespace Directions.Infrastructure.ExternalApi.MapQuest.MapQuestModels;

// These classes were generated from the API request

public class DirectionsRequest
{
    public string[] Locations { get; set; } = new string[2];
    public DirectionsRequestOptions? Options { get; set; } = new();
}

public class DirectionsRequestOptions
{
    public string RouteType { get; set; } = "fastest";
    public string Unit { get; set; } = "k";
    public bool DoReverseGeocode { get; set; } = false;
    public string NarrativeType { get; set; } = "html";
    public bool EnhancedNarrative { get; set; } = true;
    public string Locale { get; set; } = "en_GB";
    public bool ManMaps { get; set; } = false;
    public int TimeType { get; set; } = 1;
    public int DrivingStyle { get; set; } = 2;
}

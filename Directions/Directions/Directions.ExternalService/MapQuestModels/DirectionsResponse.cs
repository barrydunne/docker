namespace Directions.ExternalService.MapQuestModels
{
    // These classes were generated from the API response

    public class DirectionsResponse
    {
        public DirectionsResponseRoute? Route { get; set; }
        public DirectionsResponseInfo? Info { get; set; }
    }

    public class DirectionsResponseRoute
    {
        public string? SessionId { get; set; }
        public int? RealTime { get; set; }
        public double? Distance { get; set; }
        public int? Time { get; set; }
        public string? FormattedTime { get; set; }
        public bool? HasHighway { get; set; }
        public bool? HasTollRoad { get; set; }
        public bool? HasBridge { get; set; }
        public bool? HasSeasonalClosure { get; set; }
        public bool? HasTunnel { get; set; }
        public bool? HasFerry { get; set; }
        public bool? HasUnpaved { get; set; }
        public bool? HasTimedRestriction { get; set; }
        public bool? HasCountryCross { get; set; }
        public List<DirectionsResponseLeg>? Legs { get; set; }
        public DirectionsResponseOptions? Options { get; set; }
        public DirectionsResponseBoundingBox? BoundingBox { get; set; }
        public string? Name { get; set; }
        public string? MaxRoutes { get; set; }
        public List<DirectionsResponseLocation>? Locations { get; set; }
        public List<int?>? LocationSequence { get; set; }
    }

    public class DirectionsResponseLeg
    {
        public int? Index { get; set; }
        public bool? HasTollRoad { get; set; }
        public bool? HasHighway { get; set; }
        public bool? HasBridge { get; set; }
        public bool? HasUnpaved { get; set; }
        public bool? HasTunnel { get; set; }
        public bool? HasSeasonalClosure { get; set; }
        public bool? HasFerry { get; set; }
        public bool? HasCountryCross { get; set; }
        public bool? HasTimedRestriction { get; set; }
        public double? Distance { get; set; }
        public int? Time { get; set; }
        public string? FormattedTime { get; set; }
        public int? OrigIndex { get; set; }
        public string? OrigNarrative { get; set; }
        public int? DestIndex { get; set; }
        public string? DestNarrative { get; set; }
        public List<DirectionsResponseManeuver>? Maneuvers { get; set; }
    }

    public class DirectionsResponseManeuver
    {
        public int? Index { get; set; }
        public double? Distance { get; set; }
        public string? Narrative { get; set; }
        public int? Time { get; set; }
        public int? Direction { get; set; }
        public string? DirectionName { get; set; }
        public List<object>? Signs { get; set; }
        public List<object>? ManeuverNotes { get; set; }
        public string? FormattedTime { get; set; }
        public string? TransportMode { get; set; }
        public DirectionsResponseStartPoint? StartPoint { get; set; }
        public int? TurnType { get; set; }
        public int? Attributes { get; set; }
        public string? IconUrl { get; set; }
        public List<string>? Streets { get; set; }
    }

    public class DirectionsResponseStartPoint
    {
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }

    public class DirectionsResponseOptions
    {
        public string? RouteType { get; set; }
        public string? Unit { get; set; }
        public bool? DoReverseGeocode { get; set; }
        public string? NarrativeType { get; set; }
        public bool? EnhancedNarrative { get; set; }
        public string? Locale { get; set; }
        public bool? ManMaps { get; set; }
        public int? TimeType { get; set; }
        public int? DrivingStyle { get; set; }
        public int? WalkingSpeed { get; set; }
        public int? HighwayEfficiency { get; set; }
        public bool? Avoids { get; set; }
        public int? Generalize { get; set; }
        public string? ShapeFormat { get; set; }
        public bool? UseTraffic { get; set; }
        public int? DateType { get; set; }
        public bool? SideOfStreetDisplay { get; set; }
    }

    public class DirectionsResponseBoundingBox
    {
        public DirectionsResponseUl? Ul { get; set; }
        public DirectionsResponseLr? Lr { get; set; }
    }

    public class DirectionsResponseUl
    {
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }

    public class DirectionsResponseLr
    {
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }

    public class DirectionsResponseLocation
    {
        public string? Street { get; set; }
        public string? AdminArea6 { get; set; }
        public string? AdminArea6Type { get; set; }
        public string? AdminArea5 { get; set; }
        public string? AdminArea5Type { get; set; }
        public string? AdminArea4 { get; set; }
        public string? AdminArea4Type { get; set; }
        public string? AdminArea3 { get; set; }
        public string? AdminArea3Type { get; set; }
        public string? AdminArea1 { get; set; }
        public string? AdminArea1Type { get; set; }
        public string? PostalCode { get; set; }
        public string? GeocodeQualityCode { get; set; }
        public string? GeocodeQuality { get; set; }
        public bool? DragPoint { get; set; }
        public string? SideOfStreet { get; set; }
        public string? LinkId { get; set; }
        public string? UnknownInput { get; set; }
        public string? Type { get; set; }
        public DirectionsResponseLatLng? LatLng { get; set; }
        public DirectionsResponseDisplayLatLng? DisplayLatLng { get; set; }
        public string? MapUrl { get; set; }
    }

    public class DirectionsResponseLatLng
    {
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }

    public class DirectionsResponseDisplayLatLng
    {
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }

    public class DirectionsResponseInfo
    {
        public int? StatusCode { get; set; }
        public DirectionsResponseCopyright? Copyright { get; set; }
        public string[]? Messages { get; set; }
    }

    public class DirectionsResponseCopyright
    {
        public string? Text { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageAltText { get; set; }
    }
}

namespace Microservices.Shared.Events
{
    /// <summary>
    /// A single step in the directions between locations.
    /// </summary>
    /// <param name="Description">The information for this step of the directions.</param>
    /// <param name="TravelTimeSeconds">The estimated seconds travel time.</param>
    /// <param name="DistanceKm">The distance in kilometres to travel.</param>
    public record DirectionsStep(string Description, int? TravelTimeSeconds, double? DistanceKm);
}

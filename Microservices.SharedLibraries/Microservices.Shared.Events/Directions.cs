namespace Microservices.Shared.Events;

/// <summary>
/// The directions between two locations.
/// </summary>
/// <param name="IsSuccessful">Whether the journey directions were obtained successfully.</param>
/// <param name="TravelTimeSeconds">The estimated seconds travel time.</param>
/// <param name="DistanceKm">The distance in kilometres to travel.</param>
/// <param name="Steps">The individual steps in the directions between locations.</param>
/// <param name="Error">The error if not successful.</param>
public record Directions(bool IsSuccessful, int? TravelTimeSeconds, double? DistanceKm, DirectionsStep[]? Steps, string? Error);

namespace Microservices.Shared.Events;

/// <summary>
/// The result of geocoding a single address.
/// </summary>
/// <param name="IsSuccessful">Whether the address was geocoded to coordinates successfully.</param>
/// <param name="Coordinates">The coordinates if successful.</param>
/// <param name="Error">The error if not successful.</param>
public record GeocodingCoordinates(bool IsSuccessful, Coordinates? Coordinates, string? Error);

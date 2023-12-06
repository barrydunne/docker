using Directions.Application.ExternalApi;
using Microservices.Shared.Events;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Directions.Infrastructure.ExternalApi.Dummy;

/// <summary>
/// Uses dummy directions.
/// </summary>
public class DummyApi : IExternalApi
{
    private readonly ILogger _logger;

    private static readonly ConcurrentDictionary<string, Microservices.Shared.Events.Directions> _knownDirections = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DummyApi"/> class.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    public DummyApi(ILogger<DummyApi> logger) => _logger = logger;

    /// <summary>
    /// Add some fixed directions for specific coordinates.
    /// </summary>
    /// <param name="startingCoordinates">The starting location.</param>
    /// <param name="destinationCoordinates">The destination location.</param>
    /// <param name="directions">The known directions to return for this journey.</param>
    public static void AddDirections(Coordinates startingCoordinates, Coordinates destinationCoordinates, Microservices.Shared.Events.Directions directions) => _knownDirections[GetKey(startingCoordinates, destinationCoordinates)] = directions;

    private static string GetKey(Coordinates startingCoordinates, Coordinates destinationCoordinates) => $"{startingCoordinates.Latitude},{startingCoordinates.Longitude} to {destinationCoordinates.Latitude},{destinationCoordinates.Longitude}";

    /// <inheritdoc/>
    public Task<Microservices.Shared.Events.Directions> GetDirectionsAsync(Coordinates startingCoordinates, Coordinates destinationCoordinates, Guid correlationId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Returning directions. [{CorrelationId}]", correlationId);
        if (!_knownDirections.TryGetValue(GetKey(startingCoordinates, destinationCoordinates), out var directions))
        {
            directions = new Microservices.Shared.Events.Directions(
                true,
                930,
                13.6,
                new[]
                {
                    new DirectionsStep("Head southeast on Silver Creek Ct. Go for 60 m.", 10, 0.1),
                    new DirectionsStep("Turn right onto Clear Creek Cir. Go for 192 m.", 29, 0.2),
                    new DirectionsStep("Turn right onto Woodcrest Way. Go for 1.4 km.", 200, 1.4),
                    new DirectionsStep("Turn left onto US Highway 27 (US-27 S). Go for 2.0 km.", 109, 2),
                    new DirectionsStep("Take slip road onto W Irlo Bronson Memorial Hwy (US-192 E) toward Kissimmee. Go for 9.0 km.", 428, 9),
                    new DirectionsStep("Turn right onto Entry Point Blvd. Go for 361 m.", 51, 0.4),
                    new DirectionsStep("Turn right onto Funie Steed Rd. Go for 273 m.", 38, 0.3),
                    new DirectionsStep("Turn right onto Lake Dr. Go for 319 m.", 64, 0.3),
                    new DirectionsStep("Arrive at Lake Dr. Your destination is on the left.", 0, 0)
                },
                null);
        }
        return Task.FromResult(directions);
    }
}

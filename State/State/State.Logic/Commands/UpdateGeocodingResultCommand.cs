using AspNet.KickStarter.CQRS.Abstractions.Commands;
using Microservices.Shared.Events;

namespace State.Logic.Commands
{
    /// <summary>
    /// Record the result of geocoding and publish the appropriate event.
    /// </summary>
    /// <param name="JobId">The correlation id for tracking this job.</param>
    /// <param name="StartingCoordinates">The geocoding result for the starting location for the job.</param>
    /// <param name="DestinationCoordinates">The geocoding result for the destination location for the job.</param>
    public record UpdateGeocodingResultCommand(Guid JobId, GeocodingCoordinates StartingCoordinates, GeocodingCoordinates DestinationCoordinates) : ICommand;
}

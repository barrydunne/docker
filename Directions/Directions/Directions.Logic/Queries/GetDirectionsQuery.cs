using AspNet.KickStarter.CQRS.Abstractions.Queries;
using CSharpFunctionalExtensions;
using Microservices.Shared.Events;

namespace Directions.Logic.Queries
{
    /// <summary>
    /// Get the directions for a journey.
    /// </summary>
    /// <param name="JobId">The correlation id to include in logging when handling this query.</param>
    /// <param name="StartingCoordinates">The starting location for the job.</param>
    /// <param name="DestinationCoordinates">The destination location for the job.</param>
    public record GetDirectionsQuery(Guid JobId, Coordinates StartingCoordinates, Coordinates DestinationCoordinates) : IQuery<Result<Microservices.Shared.Events.Directions>>;
}

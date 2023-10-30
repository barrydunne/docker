using AspNet.KickStarter.CQRS.Abstractions.Queries;
using CSharpFunctionalExtensions;
using Microservices.Shared.Events;

namespace Geocoding.Logic.Queries
{
    /// <summary>
    /// Get the coordinates for an address.
    /// </summary>
    /// <param name="JobId">The correlation id to include in logging when handling this query.</param>
    /// <param name="Address">The address to geocode.</param>
    public record GetAddressCoordinatesQuery(Guid JobId, string Address) : IQuery<Result<Coordinates>>;
}

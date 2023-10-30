using AspNet.KickStarter.CQRS.Abstractions.Queries;
using CSharpFunctionalExtensions;
using Microservices.Shared.Events;

namespace Imaging.Logic.Queries
{
    /// <summary>
    /// Get the image for a location.
    /// </summary>
    /// <param name="JobId">The correlation id to include in logging when handling this query.</param>
    /// <param name="Address">The target location address for the image.</param>
    /// <param name="Coordinates">The target location coordinates for the image.</param>
    public record GetImageUrlQuery(Guid JobId, string Address, Coordinates Coordinates) : IQuery<Result<string?>>;
}

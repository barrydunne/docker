using AspNet.KickStarter.CQRS.Abstractions.Commands;
using Microservices.Shared.Events;

namespace State.Logic.Commands
{
    /// <summary>
    /// Record the directions result and check if the job is complete.
    /// </summary>
    /// <param name="JobId">The correlation id for tracking this job.</param>
    /// <param name="Directions">The directions between the locations.</param>
    public record UpdateDirectionsResultCommand(Guid JobId, Directions Directions) : ICommand;
}

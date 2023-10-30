using AspNet.KickStarter.CQRS.Abstractions.Commands;
using Microservices.Shared.Events;

namespace Weather.Logic.Commands
{
    /// <summary>
    /// Perform weather on two addresses.
    /// </summary>
    /// <param name="JobId">The correlation id to include in logging when handling this command.</param>
    /// <param name="Coordinates">The location for the job.</param>
    public record GenerateWeatherCommand(Guid JobId, Coordinates Coordinates) : ICommand;
}

using AspNet.KickStarter.CQRS.Abstractions.Commands;
using Microservices.Shared.Events;

namespace State.Logic.Commands
{
    /// <summary>
    /// Record the weather result and check if the job is complete.
    /// </summary>
    /// <param name="JobId">The correlation id for tracking this job.</param>
    /// <param name="Weather">The weather forecast at the destination.</param>
    public record UpdateWeatherResultCommand(Guid JobId, WeatherForecast Weather) : ICommand;
}

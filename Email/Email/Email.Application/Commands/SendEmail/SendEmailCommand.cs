using AspNet.KickStarter.CQRS.Abstractions.Commands;
using Microservices.Shared.Events;

namespace Email.Application.Commands.SendEmail;

/// <summary>
/// Send an email with the processing results.
/// </summary>
/// <param name="JobId">The correlation id for tracking this job.</param>
/// <param name="Email">The notification email address for the job.</param>
/// <param name="StartingAddress">The starting location for the job.</param>
/// <param name="DestinationAddress">The destination location for the job.</param>
/// <param name="Directions">The directions between the locations.</param>
/// <param name="Weather">The weather forecast at the destination.</param>
/// <param name="Imaging">The result of imaging.</param>
public record SendEmailCommand(Guid JobId, string Email, string StartingAddress, string DestinationAddress, Directions Directions, WeatherForecast Weather, ImagingResult Imaging) : ICommand;

using AspNet.KickStarter.CQRS.Abstractions.Commands;

namespace State.Application.Commands.CreateJob;

/// <summary>
/// Record that a new job has been created.
/// </summary>
/// <param name="JobId">The correlation id for tracking this job.</param>
/// <param name="StartingAddress">The starting location for the job.</param>
/// <param name="DestinationAddress">The destination location for the job.</param>
/// <param name="Email">The notification email address for the job.</param>
public record CreateJobCommand(Guid JobId, string StartingAddress, string DestinationAddress, string Email) : ICommand;

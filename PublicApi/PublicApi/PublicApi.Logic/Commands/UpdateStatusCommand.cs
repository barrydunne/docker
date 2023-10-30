using AspNet.KickStarter.CQRS.Abstractions.Commands;
using Microservices.Shared.Events;

namespace PublicApi.Logic.Commands
{
    /// <summary>
    /// Record the new job status.
    /// </summary>
    /// <param name="JobId">The correlation id for tracking this job.</param>
    /// <param name="Status">The current status of the job.</param>
    /// <param name="AdditionalInformation">Additional details relating to the job status.</param>
    public record UpdateStatusCommand(Guid JobId, JobStatus Status, string? AdditionalInformation) : ICommand;
}

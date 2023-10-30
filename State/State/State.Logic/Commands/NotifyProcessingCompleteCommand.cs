using AspNet.KickStarter.CQRS.Abstractions.Commands;
using State.Repository.Models;

namespace State.Logic.Commands
{
    /// <summary>
    /// The job status has changed..
    /// </summary>
    /// <param name="JobId">The correlation id for tracking this job.</param>
    /// <param name="IsSuccessful">Whether processing was fully successful.</param>
    /// <param name="Job">The full details for the job.</param>
    public record NotifyProcessingCompleteCommand(Guid JobId, bool IsSuccessful, Job Job) : ICommand;
}

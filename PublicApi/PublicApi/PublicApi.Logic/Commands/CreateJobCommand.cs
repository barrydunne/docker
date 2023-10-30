using AspNet.KickStarter.CQRS.Abstractions.Commands;

namespace PublicApi.Logic.Commands
{
    /// <summary>
    /// Create a new job for processing.
    /// </summary>
    /// <param name="IdempotencyKey">The idempotency key for this command.</param>
    /// <param name="StartingAddress">The starting location for the job.</param>
    /// <param name="DestinationAddress">The destination location for the job.</param>
    /// <param name="Email">The notification email address for the job.</param>
    public record CreateJobCommand(string IdempotencyKey, string StartingAddress, string DestinationAddress, string Email) : ICommand<Guid>;
}

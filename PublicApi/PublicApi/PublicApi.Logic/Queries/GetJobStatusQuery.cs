using AspNet.KickStarter.CQRS.Abstractions.Queries;
using PublicApi.Repository.Models;

namespace PublicApi.Logic.Queries
{
    /// <summary>
    /// Get the current status of an existing job.
    /// </summary>
    /// <param name="JobId">The id of the existing job.</param>
    public record GetJobStatusQuery(Guid JobId) : IQuery<Job?>;
}

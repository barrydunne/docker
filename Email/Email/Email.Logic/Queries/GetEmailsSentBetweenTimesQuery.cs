using AspNet.KickStarter.CQRS.Abstractions.Queries;
using Email.Repository.Models;

namespace Email.Logic.Queries
{
    /// <summary>
    /// Gets all emails sent between specific times.
    /// </summary>
    /// <param name="FromTime">The earliest sent email to search for.</param>
    /// <param name="ToTime">The latest sent email to search for.</param>
    /// <param name="PageSize">The page number of results to return. Starting with 1.</param>
    /// <param name="PageNumber">The number of results to return per page.</param>
    public record GetEmailsSentBetweenTimesQuery(DateTime FromTime, DateTime ToTime, int PageSize, int PageNumber) : IQuery<List<SentEmail>>;
}

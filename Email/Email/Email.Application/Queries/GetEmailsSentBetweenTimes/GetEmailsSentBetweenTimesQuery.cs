using AspNet.KickStarter.CQRS.Abstractions.Queries;
using Email.Application.Models;

namespace Email.Application.Queries.GetEmailsSentBetweenTimes;

/// <summary>
/// Gets all emails sent between specific times.
/// </summary>
/// <param name="FromTime">The earliest sent email to search for.</param>
/// <param name="ToTime">The latest sent email to search for.</param>
/// <param name="PageSize">The page number of results to return. Starting with 1.</param>
/// <param name="PageNumber">The number of results to return per page.</param>
public record GetEmailsSentBetweenTimesQuery(DateTimeOffset FromTime, DateTimeOffset ToTime, int PageSize, int PageNumber) : IQuery<List<SentEmail>>;

using AspNet.KickStarter.CQRS.Abstractions.Queries;
using Email.Application.Models;

namespace Email.Application.Queries.GetEmailsSentToRecipient;

/// <summary>
/// Gets all emails sent to a specific recipient.
/// </summary>
/// <param name="Email">The recipient email address to search for.</param>
/// <param name="PageSize">The page number of results to return. Starting with 1.</param>
/// <param name="PageNumber">The number of results to return per page.</param>
public record GetEmailsSentToRecipientQuery(string Email, int PageSize, int PageNumber) : IQuery<List<SentEmail>>;

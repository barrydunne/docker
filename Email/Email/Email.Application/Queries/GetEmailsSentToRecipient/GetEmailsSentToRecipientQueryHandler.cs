using Email.Application.Models;
using Email.Application.Repositories;
using Microsoft.Extensions.Logging;

namespace Email.Application.Queries.GetEmailsSentToRecipient;

/// <summary>
/// Handler for the GetEmailsSentToRecipientQuery query.
/// </summary>
internal class GetEmailsSentToRecipientQueryHandler : GetEmailsBaseQueryHandler<GetEmailsSentToRecipientQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetEmailsSentToRecipientQueryHandler"/> class.
    /// </summary>
    /// <param name="emailRepository">The repository for saving and retrieving sent email information.</param>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public GetEmailsSentToRecipientQueryHandler(IEmailRepository emailRepository, IGetEmailsSentToRecipientQueryHandlerMetrics metrics, ILogger<GetEmailsSentToRecipientQueryHandler> logger) : base(emailRepository, metrics, logger) { }

    /// <inheritdoc/>
    protected override Task<List<SentEmail>> PerformQueryAsync(GetEmailsSentToRecipientQuery query, CancellationToken cancellationToken)
        => _emailRepository.GetEmailsSentToRecipientAsync(query.Email, query.PageSize * (query.PageNumber - 1), query.PageSize, cancellationToken);
}

using Email.Application.Models;
using Email.Application.Repositories;
using Microsoft.Extensions.Logging;

namespace Email.Application.Queries.GetEmailsSentBetweenTimes;

/// <summary>
/// Handler for the GetEmailsSentBetweenTimesQuery query.
/// </summary>
internal class GetEmailsSentBetweenTimesQueryHandler : GetEmailsBaseQueryHandler<GetEmailsSentBetweenTimesQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetEmailsSentBetweenTimesQueryHandler"/> class.
    /// </summary>
    /// <param name="emailRepository">The repository for saving and retrieving sent email information.</param>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public GetEmailsSentBetweenTimesQueryHandler(IEmailRepository emailRepository, IGetEmailsSentBetweenTimesQueryHandlerMetrics metrics, ILogger<GetEmailsSentBetweenTimesQueryHandler> logger) : base(emailRepository, metrics, logger) { }

    /// <inheritdoc/>
    protected override Task<List<SentEmail>> PerformQueryAsync(GetEmailsSentBetweenTimesQuery query, CancellationToken cancellationToken)
        => _emailRepository.GetEmailsSentBetweenTimesAsync(query.FromTime, query.ToTime, query.PageSize * (query.PageNumber - 1), query.PageSize, cancellationToken);
}

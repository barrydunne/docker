using AspNet.KickStarter.CQRS;
using AspNet.KickStarter.CQRS.Abstractions.Queries;
using Email.Application.Models;
using Email.Application.Queries.GetEmailsSentToRecipient;
using Email.Application.Repositories;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Email.Application.Queries;

/// <summary>
/// Base class for handlers for the GetEmailsSentXXXQuery queries.
/// </summary>
/// <typeparam name="TQuery">The type of query being handled.</typeparam>
internal abstract class GetEmailsBaseQueryHandler<TQuery> : IQueryHandler<TQuery, List<SentEmail>> where TQuery : IQuery<List<SentEmail>>
{
    /// <summary>
    /// The repository for saving and retrieving sent email information.
    /// </summary>
    protected readonly IEmailRepository _emailRepository;

    /// <summary>
    /// The metrics provider for this handler.
    /// </summary>
    protected readonly IGetEmailsBaseQueryMetrics _metrics;

    /// <summary>
    /// The logger to write to.
    /// </summary>
    protected readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetEmailsBaseQueryHandler{TQuery}"/> class.
    /// </summary>
    /// <param name="emailRepository">The repository for saving and retrieving sent email information.</param>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    protected GetEmailsBaseQueryHandler(IEmailRepository emailRepository, IGetEmailsBaseQueryMetrics metrics, ILogger logger)
    {
        _emailRepository = emailRepository;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<List<SentEmail>>> Handle(TQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Handler} handler.", nameof(GetEmailsSentToRecipientQuery));
        _metrics.IncrementCount();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var emails = await PerformQueryAsync(request, cancellationToken);
            _metrics.RecordLoadTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
            return emails;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to retrieve emails.");
        }
        return new List<SentEmail>();
    }

    /// <summary>
    /// Execute the required query to get the results.
    /// </summary>
    /// <param name="query">The query containing the search parameters.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>The <see cref="SentEmail"/> records matching the search parameters.</returns>
    protected abstract Task<List<SentEmail>> PerformQueryAsync(TQuery query, CancellationToken cancellationToken);
}

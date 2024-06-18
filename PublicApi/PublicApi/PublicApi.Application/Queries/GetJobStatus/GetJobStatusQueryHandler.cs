using AspNet.KickStarter.CQRS.Abstractions.Queries;
using AspNet.KickStarter.FunctionalResult;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using PublicApi.Application.Caching;
using PublicApi.Application.Models;
using PublicApi.Application.Repositories;
using System.Diagnostics;

namespace PublicApi.Application.Queries.GetJobStatus;

/// <summary>
/// Handler for the GetJobStatusQuery query.
/// </summary>
internal class GetJobStatusQueryHandler : IQueryHandler<GetJobStatusQuery, Job?>
{
    private readonly IJobRepository _jobRepository;
    private readonly IJobCache _jobCache;
    private readonly IGetJobStatusQueryHandlerMetrics _metrics;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetJobStatusQueryHandler"/> class.
    /// </summary>
    /// <param name="jobRepository">The repository for saving and retrieving jobs.</param>
    /// <param name="jobCache">The cache to save and retrieve jobs from in preference to using the repository.</param>
    /// <param name="metrics">The metrics provider for this handler.</param>
    /// <param name="logger">The logger to write to.</param>
    public GetJobStatusQueryHandler(IJobRepository jobRepository, IJobCache jobCache, IGetJobStatusQueryHandlerMetrics metrics, ILogger<GetJobStatusQueryHandler> logger)
    {
        _jobRepository = jobRepository;
        _jobCache = jobCache;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<Job?>> Handle(GetJobStatusQuery query, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{Handler} handler. [{CorrelationId}]", nameof(GetJobStatusQuery), query.JobId);
        _metrics.IncrementCount();

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var job = _jobCache.Get(query.JobId);
            _metrics.RecordCacheGetTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
            if (job is null)
            {
                job = await _jobRepository.GetJobByIdAsync(query.JobId, cancellationToken);
                _metrics.RecordLoadTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);

                if (job is not null)
                {
                    _jobCache.Set(job, TimeSpan.FromMinutes(10));
                    _metrics.RecordCacheSetTime(stopwatch.GetElapsedAndRestart().TotalMilliseconds);
                }
            }
            return job;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to retrieve job status. [{CorrelationId}]", query.JobId);
            return ex;
        }
    }
}

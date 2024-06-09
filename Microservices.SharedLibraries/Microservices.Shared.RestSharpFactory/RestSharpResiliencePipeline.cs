using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using RestSharp;
using System.Net;

namespace Microservices.Shared.RestSharpFactory;

/// <inheritdoc/>
public class RestSharpResiliencePipeline : IRestSharpResiliencePipeline
{
    private readonly ResiliencePipeline<RestResponse> _pipeline;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RestSharpResiliencePipeline"/> class.
    /// </summary>
    /// <param name="logger">The logger to write events to.</param>
    public RestSharpResiliencePipeline(ILogger<RestSharpResiliencePipeline> logger)
    {
        _logger = logger;

        _pipeline = new ResiliencePipelineBuilder<RestResponse>()
            .AddRetry(new RetryStrategyOptions<RestResponse>
            {
                ShouldHandle = new PredicateBuilder<RestResponse>()
                    .HandleResult(static _ => _.StatusCode != HttpStatusCode.OK),
                MaxRetryAttempts = 5,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                OnRetry = args =>
                {
                    _logger.LogWarning("REST failure [{Status}]. Retry attempt {RetryAttempt} after a delay of {Delay}ms", args.Outcome.Result?.StatusCode, args.AttemptNumber + 1, args.RetryDelay);
                    return ValueTask.CompletedTask;
                }
            })
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions<RestResponse>()
            {
                ShouldHandle = new PredicateBuilder<RestResponse>()
                    .HandleResult(static _ => _.StatusCode != HttpStatusCode.OK),
                FailureRatio = 0.5d,
                SamplingDuration = TimeSpan.FromSeconds(10),
                MinimumThroughput = 5,
                BreakDuration = TimeSpan.FromSeconds(30),
                OnClosed = _ =>
                {
                    _logger.LogInformation("REST circuit breaker closed.");
                    return ValueTask.CompletedTask;
                },
                OnHalfOpened = _ =>
                {
                    _logger.LogInformation("REST circuit breaker half open.");
                    return ValueTask.CompletedTask;
                },
                OnOpened = _ =>
                {
                    _logger.LogWarning("REST circuit breaker opened.");
                    return ValueTask.CompletedTask;
                },
            })
            .Build();
    }

    /// <inheritdoc/>
    public ResiliencePipeline<RestResponse> GetPipeline() => _pipeline;
}

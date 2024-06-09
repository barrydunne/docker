using AspNet.KickStarter;
using AspNet.KickStarter.CQRS;
using Imaging.Api;
using Imaging.Api.BackgroundServices;
using Imaging.Application.Commands.SaveImage;
using Microservices.Shared.CloudFiles;
using Microservices.Shared.CloudSecrets;
using Microservices.Shared.Events;
using Microservices.Shared.Utilities;
using OpenTelemetry.Trace;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using System.Net;

await new ApiBuilder()
    .WithSerilog(msg => Console.WriteLine($"Serilog: {msg}"))
    .WithSwagger()
    .WithHealthHandler()
    .WithServices(IoC.RegisterServices)
    .WithEndpoints(Endpoints.Map)
    .WithOpenTelemetry(
        prometheusPort: 8081,
        configureTraceBuilder: _ => _
            .AddAWSInstrumentation()
            .AddSource(CloudFiles.ActivitySourceName)
            .AddSource(CloudSecrets.ActivitySourceName))
    .WithAdditionalConfiguration(_ => _.Services
        .AddQueueToCommandProcessor<LocationsReadyEvent, SaveImageCommand, Result, LocationsReadyEventProcessor>()
        .AddHttpClient("resilient")
            .AddPolicyHandler(RetryPolicy))
    .WithMappings(Mappings.Map)
    .Build(args)
    .RunAsync();

IAsyncPolicy<HttpResponseMessage> RetryPolicy(IServiceProvider services, HttpRequestMessage request) =>
    HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(_ => _.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryAsync(
            sleepDurations: Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5),
            onRetry: (outcome, timespan, retryAttempt, _) =>
                services.GetService<ILogger<HttpClient>>()?.LogWarning("HTTP failure [{Status}]. Retry attempt {RetryAttempt} after a delay of {Delay}ms", outcome.Result?.StatusCode, retryAttempt, timespan.TotalMilliseconds));

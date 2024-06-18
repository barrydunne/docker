using AspNet.KickStarter;
using AspNet.KickStarter.FunctionalResult;
using Microservices.Shared.CloudSecrets;
using Microservices.Shared.Events;
using Microservices.Shared.Utilities;
using OpenTelemetry.Trace;
using State.Api;
using State.Api.BackgroundServices;
using State.Application.Commands.CreateJob;
using State.Application.Commands.UpdateDirectionsResult;
using State.Application.Commands.UpdateGeocodingResult;
using State.Application.Commands.UpdateImagingResult;
using State.Application.Commands.UpdateWeatherResult;

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
            .AddSource(CloudSecrets.ActivitySourceName))
    .WithAdditionalConfiguration(_ => _.Services
        .AddQueueToCommandProcessor<JobCreatedEvent, CreateJobCommand, Result, JobCreatedEventProcessor>()
        .AddQueueToCommandProcessor<GeocodingCompleteEvent, UpdateGeocodingResultCommand, Result, GeocodingCompleteEventProcessor>()
        .AddQueueToCommandProcessor<DirectionsCompleteEvent, UpdateDirectionsResultCommand, Result, DirectionsCompleteEventProcessor>()
        .AddQueueToCommandProcessor<WeatherCompleteEvent, UpdateWeatherResultCommand, Result, WeatherCompleteEventProcessor>()
        .AddQueueToCommandProcessor<ImagingCompleteEvent, UpdateImagingResultCommand, Result, ImagingCompleteEventProcessor>())
    .WithMappings(Mappings.Map)
    .Build(args)
    .RunAsync();

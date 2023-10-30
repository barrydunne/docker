using AspNet.KickStarter;
using CSharpFunctionalExtensions;
using Microservices.Shared.Events;
using Microservices.Shared.Utilities;
using State.Api;
using State.Api.BackgroundServices;
using State.Logic.Commands;
using State.Logic.Metrics;

new ApiBuilder()
    .WithSerilog(msg => Console.WriteLine($"Serilog: {msg}"))
    .WithSwagger()
    .WithServices(IoC.RegisterServices)
    .WithEndpoints(Endpoints.Map)
    .WithMetrics(8081)
    .WithAdditionalConfiguration(_ => _.Services
        .RegisterLogicMetrics()
        .AddQueueToCommandProcessor<JobCreatedEvent, CreateJobCommand, Result, JobCreatedEventProcessor>()
        .AddQueueToCommandProcessor<GeocodingCompleteEvent, UpdateGeocodingResultCommand, Result, GeocodingCompleteEventProcessor>()
        .AddQueueToCommandProcessor<DirectionsCompleteEvent, UpdateDirectionsResultCommand, Result, DirectionsCompleteEventProcessor>()
        .AddQueueToCommandProcessor<WeatherCompleteEvent, UpdateWeatherResultCommand, Result, WeatherCompleteEventProcessor>()
        .AddQueueToCommandProcessor<ImagingCompleteEvent, UpdateImagingResultCommand, Result, ImagingCompleteEventProcessor>())
    .Build(args)
    .Run();

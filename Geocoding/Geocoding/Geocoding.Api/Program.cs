using AspNet.KickStarter;
using CSharpFunctionalExtensions;
using Geocoding.Api;
using Geocoding.Api.BackgroundServices;
using Geocoding.Logic.Commands;
using Geocoding.Logic.Metrics;
using Microservices.Shared.Events;
using Microservices.Shared.Utilities;

new ApiBuilder()
    .WithSerilog(msg => Console.WriteLine($"Serilog: {msg}"))
    .WithSwagger()
    .WithServices(IoC.RegisterServices)
    .WithEndpoints(Endpoints.Map)
    .WithMetrics(8081)
    .WithAdditionalConfiguration(_ => _.Services
        .RegisterLogicMetrics()
        .AddQueueToCommandProcessor<JobCreatedEvent, GeocodeAddressesCommand, Result, JobCreatedEventProcessor>())
    .Build(args)
    .Run();

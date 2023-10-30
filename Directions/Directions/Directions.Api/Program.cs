using AspNet.KickStarter;
using CSharpFunctionalExtensions;
using Directions.Api;
using Directions.Api.BackgroundServices;
using Directions.Logic.Commands;
using Directions.Logic.Metrics;
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
        .AddQueueToCommandProcessor<LocationsReadyEvent, GenerateDirectionsCommand, Result, LocationsReadyEventProcessor>())
    .Build(args)
    .Run();

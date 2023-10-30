using AspNet.KickStarter;
using CSharpFunctionalExtensions;
using Microservices.Shared.Events;
using Microservices.Shared.Utilities;
using Weather.Api;
using Weather.Api.BackgroundServices;
using Weather.Logic.Commands;
using Weather.Logic.Metrics;

new ApiBuilder()
    .WithSerilog(msg => Console.WriteLine($"Serilog: {msg}"))
    .WithSwagger()
    .WithServices(IoC.RegisterServices)
    .WithEndpoints(Endpoints.Map)
    .WithMetrics(8081)
    .WithAdditionalConfiguration(_ => _.Services
        .RegisterLogicMetrics()
        .AddQueueToCommandProcessor<LocationsReadyEvent, GenerateWeatherCommand, Result, LocationsReadyEventProcessor>())
    .Build(args)
    .Run();

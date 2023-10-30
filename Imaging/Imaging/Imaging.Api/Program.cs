using AspNet.KickStarter;
using CSharpFunctionalExtensions;
using Imaging.Api;
using Imaging.Api.BackgroundServices;
using Imaging.Logic.Commands;
using Imaging.Logic.Metrics;
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
        .AddHttpClient()
        .AddQueueToCommandProcessor<LocationsReadyEvent, SaveImageCommand, Result, LocationsReadyEventProcessor>())
    .Build(args)
    .Run();

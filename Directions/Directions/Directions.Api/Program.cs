using AspNet.KickStarter;
using AspNet.KickStarter.CQRS;
using Directions.Api;
using Directions.Api.BackgroundServices;
using Directions.Application.Commands.GenerateDirections;
using Microservices.Shared.Events;
using Microservices.Shared.Utilities;

await new ApiBuilder()
    .WithSerilog(msg => Console.WriteLine($"Serilog: {msg}"))
    .WithSwagger()
    .WithHealthHandler()
    .WithServices(IoC.RegisterServices)
    .WithEndpoints(Endpoints.Map)
    .WithOpenTelemetry(8081)
    .WithAdditionalConfiguration(_ => _.Services
    .AddQueueToCommandProcessor<LocationsReadyEvent, GenerateDirectionsCommand, Result, LocationsReadyEventProcessor>())
    .WithMappings(Mappings.Map)
    .Build(args)
    .RunAsync();

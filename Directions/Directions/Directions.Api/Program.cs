using AspNet.KickStarter;
using AspNet.KickStarter.CQRS;
using Directions.Api;
using Directions.Api.BackgroundServices;
using Directions.Application.Commands.GenerateDirections;
using Microservices.Shared.CloudSecrets;
using Microservices.Shared.Events;
using Microservices.Shared.Utilities;
using OpenTelemetry.Trace;

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
    .AddQueueToCommandProcessor<LocationsReadyEvent, GenerateDirectionsCommand, Result, LocationsReadyEventProcessor>())
    .WithMappings(Mappings.Map)
    .Build(args)
    .RunAsync();

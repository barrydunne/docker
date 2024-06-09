using AspNet.KickStarter;
using AspNet.KickStarter.CQRS;
using Geocoding.Api;
using Geocoding.Api.BackgroundServices;
using Geocoding.Application.Commands.GeocodeAddresses;
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
        .AddQueueToCommandProcessor<JobCreatedEvent, GeocodeAddressesCommand, Result, JobCreatedEventProcessor>())
    .WithMappings(Mappings.Map)
    .Build(args)
    .RunAsync();

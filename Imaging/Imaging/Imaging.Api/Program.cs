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
        .AddHttpClient()
        .AddQueueToCommandProcessor<LocationsReadyEvent, SaveImageCommand, Result, LocationsReadyEventProcessor>())
    .WithMappings(Mappings.Map)
    .Build(args)
    .RunAsync();

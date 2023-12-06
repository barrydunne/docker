using AspNet.KickStarter;
using AspNet.KickStarter.CQRS;
using Imaging.Api;
using Imaging.Api.BackgroundServices;
using Imaging.Application.Commands.SaveImage;
using Microservices.Shared.Events;
using Microservices.Shared.Utilities;

new ApiBuilder()
    .WithSerilog(msg => Console.WriteLine($"Serilog: {msg}"))
    .WithSwagger()
    .WithHealthHandler()
    .WithServices(IoC.RegisterServices)
    .WithEndpoints(Endpoints.Map)
    .WithMetrics(8081)
    .WithAdditionalConfiguration(_ => _.Services
        .AddHttpClient()
        .AddQueueToCommandProcessor<LocationsReadyEvent, SaveImageCommand, Result, LocationsReadyEventProcessor>())
    .WithMappings(Mappings.Map)
    .Build(args)
    .Run();

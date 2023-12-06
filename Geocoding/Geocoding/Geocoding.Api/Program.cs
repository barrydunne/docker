using AspNet.KickStarter;
using AspNet.KickStarter.CQRS;
using Geocoding.Api;
using Geocoding.Api.BackgroundServices;
using Geocoding.Application.Commands.GeocodeAddresses;
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
        .AddQueueToCommandProcessor<JobCreatedEvent, GeocodeAddressesCommand, Result, JobCreatedEventProcessor>())
    .WithMappings(Mappings.Map)
    .Build(args)
    .Run();

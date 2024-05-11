using AspNet.KickStarter;
using AspNet.KickStarter.CQRS;
using Microservices.Shared.Events;
using Microservices.Shared.Utilities;
using Weather.Api;
using Weather.Api.BackgroundServices;
using Weather.Application.Commands.GenerateWeather;

await new ApiBuilder()
    .WithSerilog(msg => Console.WriteLine($"Serilog: {msg}"))
    .WithSwagger()
    .WithHealthHandler()
    .WithServices(IoC.RegisterServices)
    .WithEndpoints(Endpoints.Map)
    .WithMetrics(8081)
    .WithAdditionalConfiguration(_ => _.Services
        .AddQueueToCommandProcessor<LocationsReadyEvent, GenerateWeatherCommand, Result, LocationsReadyEventProcessor>())
    .WithMappings(Mappings.Map)
    .Build(args)
    .RunAsync();

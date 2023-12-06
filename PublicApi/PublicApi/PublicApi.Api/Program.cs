using AspNet.KickStarter;
using AspNet.KickStarter.CQRS;
using Microservices.Shared.Events;
using Microservices.Shared.Utilities;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.IdentityModel.Tokens;
using PublicApi.Api;
using PublicApi.Api.BackgroundServices;
using PublicApi.Api.Validators;
using PublicApi.Application.Commands.UpdateStatus;
using System.Text.Json.Serialization;

new ApiBuilder()
    .WithSerilog(msg => Console.WriteLine($"Serilog: {msg}"))
    .WithSwagger(useBearerToken: true)
    .WithHealthHandler()
    .WithServices(IoC.RegisterServices)
    .WithEndpoints(Endpoints.Map)
    .WithFluentValidationFromAssemblyContaining<CreateJobRequestValidator>()
    .WithMetrics(8081)
    .WithAdditionalConfiguration(builder => builder.Services
        .AddQueueToCommandProcessor<JobStatusUpdateEvent, UpdateStatusCommand, Result, JobStatusUpdateEventProcessor>()
        .Configure<JsonOptions>(_ =>
        {
            _.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            _.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        })
        .AddHttpClient()
        .AddAuthorization(_ => _.AddPolicy("PublicApiScope", _ => _.RequireAuthenticatedUser().RequireClaim("scope", "publicapi")))
        .AddAuthentication("Bearer").AddJwtBearer("Bearer", _ =>
        {
            _.Authority = builder.Configuration.GetConnectionString("ids");
            _.RequireHttpsMetadata = false;
            _.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true
            };
        }))
    .WithMappings(Mappings.Map)
    .Build(args)
    .Run();

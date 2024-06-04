using AspNet.KickStarter;
using AspNet.KickStarter.CQRS;
using Email.Api;
using Email.Api.BackgroundServices;
using Email.Api.Validators;
using Email.Application.Commands.SendEmail;
using Email.Infrastructure;
using Microservices.Shared.Events;
using Microservices.Shared.Utilities;
using OpenTelemetry.Trace;
using IoC = Email.Api.IoC;

await new ApiBuilder()
    .WithSerilog(msg => Console.WriteLine($"Serilog: {msg}"))
    .WithSwagger()
    .WithHealthHandler()
    .WithServices(IoC.RegisterServices)
    .WithEndpoints(Endpoints.Map)
    .WithFluentValidationFromAssemblyContaining<GetEmailsSentToRecipientRequestValidator>()
    .WithOpenTelemetry(
        prometheusPort: 8081,
        configureTraceBuilder: builder =>
        {
            builder.AddEntityFrameworkCoreInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                        options.SetDbStatementForStoredProcedure = true;
                    })
                   .AddSource("Microservices.Shared.CloudFiles.Ftp")
                   .AddSource("Microservices.Shared.CloudEmail.Smtp");
        })
    .WithAdditionalConfiguration(_ => _.Services
        .AddQueueToCommandProcessor<ProcessingCompleteEvent, SendEmailCommand, Result, ProcessingCompleteEventProcessor>())
    .WithMappings(Mappings.Map)
    .Build(args)
    .WaitForDb()
    .EnsureDbCreated()
    .RunAsync();

using AspNet.KickStarter;
using AspNet.KickStarter.FunctionalResult;
using Email.Api;
using Email.Api.BackgroundServices;
using Email.Api.Validators;
using Email.Application.Commands.SendEmail;
using Email.Infrastructure;
using Microservices.Shared.CloudEmail;
using Microservices.Shared.CloudFiles;
using Microservices.Shared.CloudSecrets;
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
        configureTraceBuilder: _ => _
            .AddAWSInstrumentation()
            .AddSource(CloudEmail.ActivitySourceName)
            .AddSource(CloudFiles.ActivitySourceName)
            .AddSource(CloudSecrets.ActivitySourceName)
            .AddEntityFrameworkCoreInstrumentation(options =>
            {
                options.SetDbStatementForText = true;
                options.SetDbStatementForStoredProcedure = true;
            }))
    .WithAdditionalConfiguration(_ => _.Services
        .AddQueueToCommandProcessor<ProcessingCompleteEvent, SendEmailCommand, Result, ProcessingCompleteEventProcessor>())
    .WithMappings(Mappings.Map)
    .Build(args)
    .WaitForDb()
    .EnsureDbCreated()
    .RunAsync();

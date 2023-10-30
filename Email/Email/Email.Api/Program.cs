using AspNet.KickStarter;
using CSharpFunctionalExtensions;
using Email.Api;
using Email.Api.BackgroundServices;
using Email.Api.Validators;
using Email.Logic.Commands;
using Email.Logic.Metrics;
using Email.Repository;
using Microservices.Shared.CloudSecrets.SecretsManager;
using Microservices.Shared.Events;
using Microservices.Shared.Utilities;

new ApiBuilder()
    .WithSerilog(msg => Console.WriteLine($"Serilog: {msg}"))
    .WithSwagger()
    .WithServices(IoC.RegisterServices)
    .WithEndpoints(Endpoints.Map)
    .WithFluentValidationFromAssemblyContaining<GetEmailsSentToRecipientRequestValidator>()
    .WithMetrics(8081)
    .WithAdditionalConfiguration(_ => _.Services
        .RegisterLogicMetrics()
        .AddHttpClient()
        .AddMySqlDbContext<EmailRepositoryDbContext>(_.Configuration.GetSection("ConnectionStrings").ApplySecret(_, "mysql", "email", "mysql.connectionstring")["mysql"]!)
        .AddQueueToCommandProcessor<ProcessingCompleteEvent, SendEmailCommand, Result, ProcessingCompleteEventProcessor>())
    .Build(args)
    .WaitForMySql()
    .EnsureDbCreated()
    .Run();

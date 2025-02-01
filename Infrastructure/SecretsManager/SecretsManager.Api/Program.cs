using AspNet.KickStarter;
using SecretsManager.Api;

await new ApiBuilder()
    .WithSerilog(msg => Console.WriteLine($"Serilog: {msg}"))
    .WithSwagger(title: "Secrets Manager API")
    .WithHealthHandler()
    .WithServices(IoC.RegisterServices)
    .WithEndpoints(Endpoints.Map)
    .Build(args)
    .RunAsync();

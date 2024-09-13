using AspNet.KickStarter;
using Duende.IdentityServer.Configuration;
using IdentityServer.Api;

await new ApiBuilder()
    .WithSerilog(msg => Console.WriteLine($"Serilog: {msg}"))
    .WithAdditionalConfiguration(_ => _.Services
        .Configure<IdentityServerOptions>(_ => _.IssuerUri = "http://ids.microservices-infrastructure:8080")
        .AddIdentityServer()
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddDeveloperSigningCredential())
    .WithApplicationConfiguration(_ => _.UseIdentityServer())
    .Build(args)
    .RunAsync();

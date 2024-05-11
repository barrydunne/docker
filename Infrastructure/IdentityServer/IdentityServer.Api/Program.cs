using AspNet.KickStarter;
using IdentityServer.Api;
using IdentityServer4.Configuration;

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

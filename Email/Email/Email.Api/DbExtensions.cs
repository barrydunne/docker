using Email.Repository;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;

namespace Email.Api
{
    /// <summary>
    /// Provides extension methods related to the database configuration.
    /// </summary>
    internal static class DbExtensions
    {
        /// <summary>
        /// Register the DbContext type with MySql connections to the given connection string.
        /// </summary>
        /// <typeparam name="TContext">The DbContext to register.</typeparam>
        /// <param name="services">The services to register with.</param>
        /// <param name="connectionString">The MySql connection string.</param>
        /// <returns>The original service collection.</returns>
        public static IServiceCollection AddMySqlDbContext<TContext>(this IServiceCollection services, string connectionString) where TContext : DbContext
        {
            services.AddDbContext<TContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                options.EnableSensitiveDataLogging();
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            return services;
        }

        /// <summary>
        /// Ensure that the database schema is created.
        /// </summary>
        /// <param name="app">The web application.</param>
        /// <param name="timeout">The time to wait for the db connection to become available.</param>
        /// <returns>The original web application.</returns>
        public static WebApplication WaitForMySql(this WebApplication app, TimeSpan? timeout = null)
        {
            timeout ??= TimeSpan.FromSeconds(120);
            var end = DateTime.UtcNow.Add(timeout.Value);
            while (DateTime.UtcNow < end)
            {
                try
                {
                    using var scope = app.Services.CreateScope();
                    using var context = scope.ServiceProvider.GetService<EmailRepositoryDbContext>()!;
                    var connectionString = context.Database.GetConnectionString();
                    if (connectionString is not null)
                    {
                        var kvp = connectionString.Split(';').Select(_ => _.Split('=')).Where(_ => _.Length == 2).ToDictionary(_ => _[0].Trim(), _ => _[1].Trim(), StringComparer.OrdinalIgnoreCase);
                        if (kvp.TryGetValue("Server", out var server))
                        {
                            var port = int.Parse(kvp.GetValueOrDefault("Port", "3306"));
                            using var tcp = new TcpClient();
                            Console.WriteLine($"Testing connection to MySql at {server}:{port}");
                            tcp.Connect(server, port);
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("MySQL connection unavailable, waiting 2 seconds before trying again.");
                }
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            return app;
        }

        /// <summary>
        /// Ensure that the database schema is created.
        /// </summary>
        /// <param name="app">The web application.</param>
        /// <returns>The original web application.</returns>
        public static WebApplication EnsureDbCreated(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            using var context = scope.ServiceProvider.GetService<EmailRepositoryDbContext>()!;
            context.Database.EnsureCreated();
            return app;
        }
    }
}

using Ardalis.GuardClauses;
using AspNet.KickStarter.FunctionalResult;
using MediatR;
using Microservices.Shared.CloudSecrets;
using Microservices.Shared.CloudSecrets.Aws;
using Microservices.Shared.CloudSecrets.SecretsManager;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Queues.RabbitMQ;
using Microservices.Shared.RestSharpFactory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microservices.Shared.Utilities;

/// <summary>
/// A collection of helpful extension methods to reduce code duplication.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Add the default microservice dependencies.
    /// </summary>
    /// <param name="builder">The builder to add services to.</param>
    /// <returns>The original builder.</returns>
    public static IHostApplicationBuilder AddMicroserviceDependencies(this IHostApplicationBuilder builder)
    {
        // RestSharp
        builder.Services
            .AddSingleton<IRestSharpResiliencePipeline, RestSharpResiliencePipeline>() // Assumes accessing single API per application
            .AddTransient<IRestSharpClientFactory, RestSharpClientFactory>();

        // Secrets
        builder.Services
            .AddCloudServices(builder.Configuration);

        // RabbitMQ
        builder.Services
            .AddTransient(typeof(IQueue<>), typeof(RabbitMQQueue<>))
            .AddTransient<IConnectionFactory, ConnectionFactory>()
            .Configure<RabbitMQQueueOptions>(builder.Configuration.GetSection("RabbitMQQueueOptions")
                                                    .ApplySecret(builder, "User", "infrastructure", "rabbit.user")
                                                    .ApplySecret(builder, "Password", "infrastructure", "rabbit.password")
                                                    .ApplySecret(builder, "VirtualHost", "infrastructure", "rabbit.vhost"));

        return builder;
    }

    private static IServiceCollection AddCloudServices(this IServiceCollection services, IConfiguration configuration)
    {
        return Environment.GetEnvironmentVariable("Microservices.CloudProvider") == "AWS"
            ? services.AddAwsCloudServices(configuration)
            : services.AddLocalCloudServices(configuration);
    }

    private static IServiceCollection AddAwsCloudServices(this IServiceCollection services, IConfiguration configuration)
        => services.AddCloudSecretsAws(configuration);

    private static IServiceCollection AddLocalCloudServices(this IServiceCollection services, IConfiguration configuration)
        => services.AddCloudSecretsManager(configuration);

    /// <summary>
    /// Register a background queue processor.
    /// </summary>
    /// <param name="services">The collection to add services to.</param>
    /// <returns>The original services.</returns>
    /// <typeparam name="TMessage">The type of message for this queue.</typeparam>
    /// <typeparam name="TCommand">The type of command to dispatch.</typeparam>
    /// <typeparam name="TResult">The type of command result.</typeparam>
    /// <typeparam name="TProcessor">The type of processor.</typeparam>
    public static IServiceCollection AddQueueToCommandProcessor<TMessage, TCommand, TResult, TProcessor>(this IServiceCollection services)
        where TMessage : BaseEvent
        where TCommand : IRequest<TResult>
        where TResult : IResult
        where TProcessor : QueueToCommandProcessor<TMessage, TCommand, TResult>
        => services
            .AddSingleton<QueueToCommandProcessor<TMessage, TCommand, TResult>, TProcessor>()
            .AddHostedService<QueueBackgroundService<TMessage, TCommand, TResult>>();

    /// <summary>
    /// Get the current elapsed time on the stopwatch and then restart it.
    /// </summary>
    /// <param name="stopwatch">The stopwatch to restart.</param>
    /// <returns>The elapsed time on the stopwatch.</returns>
    public static TimeSpan GetElapsedAndRestart(this Stopwatch stopwatch)
    {
        var elapsed = stopwatch.Elapsed;
        stopwatch.Restart();
        return elapsed;
    }

    /// <summary>
    /// Get a <see cref="Result"/> with success or failure message from a complete <see cref="Task"/>.
    /// </summary>
    /// <typeparam name="T">The type of Result to return.</typeparam>
    /// <param name="task">The complete Task.</param>
    /// <returns>A <see cref="Result"/> that is either returned from the task, or if the task is faulted then a failed <see cref="Result"/> containing the task exception message.</returns>
    public static Result<T> GetTaskResult<T>(this Task<Result<T>> task)
    {
        if (task.IsFaulted)
        {
            Error error = task.Exception;
            if (task.Exception.InnerException is not null)
                error = task.Exception.InnerException;
            return error;
        }
        return task.Result;
    }

    /// <summary>
    /// Guards against an empty array.
    /// </summary>
    /// <param name="guardClause">The guard clause to extend this method to.</param>
    /// <param name="input">The input array.</param>
    /// <param name="parameterName">The name of the parameter being guarded.</param>
    /// <param name="message">An optional message to override the default error message.</param>
    /// <returns>The array.</returns>
    /// <exception cref="ArgumentException">If the array is empty.</exception>
    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Parameter is the extended type.")]
    public static Array Empty(this IGuardClause guardClause, Array input, string parameterName, string? message = null)
    {
        if (input.Length == 0)
            throw new ArgumentException(message ?? $"Required input {parameterName} was empty.", parameterName);
        return input;
    }
}

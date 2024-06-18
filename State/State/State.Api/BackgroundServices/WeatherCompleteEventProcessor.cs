using AspNet.KickStarter;
using AspNet.KickStarter.FunctionalResult;
using Mapster;
using Microservices.Shared.Events;
using Microservices.Shared.Queues;
using Microservices.Shared.Utilities;
using State.Application.Commands.UpdateWeatherResult;

namespace State.Api.BackgroundServices;

/// <summary>
/// Processes <see cref="WeatherCompleteEvent"/> messages.
/// </summary>
internal class WeatherCompleteEventProcessor : QueueToCommandProcessor<WeatherCompleteEvent, UpdateWeatherResultCommand, Result>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherCompleteEventProcessor"/> class.
    /// </summary>
    /// <param name="queue">The queue being processed.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to use to create scoped instances.</param>
    /// <param name="traceActivity">The trace activity source.</param>
    /// <param name="logger">The logger to write to.</param>
    public WeatherCompleteEventProcessor(IQueue<WeatherCompleteEvent> queue, IServiceProvider serviceProvider, ITraceActivity traceActivity, ILogger<WeatherCompleteEventProcessor> logger) : base(queue, serviceProvider, traceActivity, logger) { }

    /// <inheritdoc/>
    protected override UpdateWeatherResultCommand CreateCommand(WeatherCompleteEvent message) => message.Adapt<UpdateWeatherResultCommand>();
}

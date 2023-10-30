namespace Microservices.Shared.Events
{
    /// <summary>
    /// The base for all events.
    /// </summary>
    /// <param name="JobId">The shared correlation id used to trace job activity.</param>
    /// <param name="CreatedUtc">The UTC time the message was created.</param>
    public abstract record BaseEvent(Guid JobId, DateTime CreatedUtc);
}

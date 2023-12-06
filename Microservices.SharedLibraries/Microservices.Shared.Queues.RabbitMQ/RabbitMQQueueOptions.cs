namespace Microservices.Shared.Queues.RabbitMQ;

/// <summary>
/// The connection options for RabbitMQ.
/// </summary>
public class RabbitMQQueueOptions
{
    /// <summary>
    /// Gets or sets the available nodes for the RabbitMQ connection.
    /// </summary>
    public string[] Nodes { get; set; } = new[] { "localhost:5672" };

    /// <summary>
    /// Gets or sets the username for the RabbitMQ connection.
    /// </summary>
    public string User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the password for the RabbitMQ connection.
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// Gets or sets the vhost for the RabbitMQ connection.
    /// </summary>
    public string VirtualHost { get; set; } = null!;

    /// <summary>
    /// Gets or sets the hostname for the RabbitMQ connection.
    /// </summary>
    public string SubscriberSuffix { get; set; } = null!;

    /// <summary>
    /// Gets or sets the delay before a rejected message will be retried. Retries are only supported on non-transient queue subscriptions.
    /// </summary>
    public int RetryDelayMilliseconds { get; set; } = 0;
}

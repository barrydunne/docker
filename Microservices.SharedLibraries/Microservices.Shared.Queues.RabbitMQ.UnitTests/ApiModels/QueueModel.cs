namespace Microservices.Shared.Queues.RabbitMQ.UnitTests.ApiModels
{
    internal record QueueModel(string Queue, bool Durable, bool Exclusive, bool AutoDelete, IDictionary<string, object>? Arguments);
}

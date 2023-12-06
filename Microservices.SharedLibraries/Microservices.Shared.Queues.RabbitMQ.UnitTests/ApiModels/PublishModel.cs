using RabbitMQ.Client;

namespace Microservices.Shared.Queues.RabbitMQ.UnitTests.ApiModels;

internal record PublishModel(string Exchange, string RoutingKey, bool Mandatory, IBasicProperties BasicProperties, ReadOnlyMemory<byte> Body);

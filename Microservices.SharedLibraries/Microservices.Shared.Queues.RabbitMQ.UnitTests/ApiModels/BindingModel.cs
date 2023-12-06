namespace Microservices.Shared.Queues.RabbitMQ.UnitTests.ApiModels;

internal record BindingModel(string Queue, string Exchange, string RoutingKey);

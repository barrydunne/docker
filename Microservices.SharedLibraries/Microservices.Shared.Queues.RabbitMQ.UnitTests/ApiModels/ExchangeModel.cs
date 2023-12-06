namespace Microservices.Shared.Queues.RabbitMQ.UnitTests.ApiModels;

internal record ExchangeModel(string Exchange, string Type, bool Durable, bool AutoDelete, IDictionary<string, object> Arguments);

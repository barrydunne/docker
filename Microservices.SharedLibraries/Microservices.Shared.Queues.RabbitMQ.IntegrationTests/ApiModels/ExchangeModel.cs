using System.Text.Json.Serialization;

namespace Microservices.Shared.Queues.RabbitMQ.IntegrationTests.ApiModels
{
    internal class ExchangeModel
    {
        [JsonPropertyName("vhost")]
        public string? VHost { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("durable")]
        public bool? Durable { get; set; }

        [JsonPropertyName("auto_delete")]
        public bool? AutoDelete { get; set; }
    }
}

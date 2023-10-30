using System.Text.Json.Serialization;

namespace Microservices.Shared.Queues.RabbitMQ.IntegrationTests.ApiModels
{
    internal class BindingModel
    {
        [JsonPropertyName("vhost")]
        public string? VHost { get; set; }

        [JsonPropertyName("source")]
        public string? Source { get; set; }

        [JsonPropertyName("destination")]
        public string? Destination { get; set; }

        [JsonPropertyName("destination_type")]
        public string? DestinationType { get; set; }

        [JsonPropertyName("routing_key")]
        public string? RoutingKey { get; set; }
    }
}

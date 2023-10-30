using Microservices.Shared.Events;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Geocoding.ExternalService
{
    /// <summary>
    /// Uses dummy geocoding.
    /// </summary>
    public class Dummy : IExternalService
    {
        private readonly ILogger _logger;
        private readonly Coordinates[] _coordinates;

        private static readonly ConcurrentDictionary<string, Coordinates> _knownCoordinates = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Dummy"/> class.
        /// </summary>
        /// <param name="logger">The logger to write to.</param>
        public Dummy(ILogger<Dummy> logger)
        {
            _logger = logger;
            _coordinates = new[]
            {
                new Coordinates(28.3643807M, -81.6681109M),
                new Coordinates(28.3359915M, -81.5957835M)
            };
        }

        /// <summary>
        /// Add some fixed coordinates for a specific address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="coordinates">The coordinates for this address.</param>
        public static void AddCoordinates(string address, Coordinates coordinates) => _knownCoordinates[address] = coordinates;

        /// <inheritdoc/>
        public Task<Coordinates> GetCoordinatesAsync(string address, Guid correlationId, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Returning coordinates. [{CorrelationId}]", correlationId);
            if (!_knownCoordinates.TryGetValue(address, out var coordinates))
                coordinates = _coordinates[Random.Shared.Next(0, _coordinates.Length)];
            return Task.FromResult(coordinates);
        }
    }
}

using Microservices.Shared.Events;

namespace Geocoding.ExternalService
{
    /// <summary>
    /// Performs geocoding of an address to coordinates.
    /// </summary>
    public interface IExternalService
    {
        /// <summary>
        /// Performs geocoding of an address to coordinates.
        /// </summary>
        /// <param name="address">The address to geocode.</param>
        /// <param name="correlationId">The correlation id to include in all logging.</param>
        /// <param name="cancellationToken">The token to cancel the operation.</param>
        /// <returns>The coordinates.</returns>
        /// <exception cref="GeocodingException">If coordinates could not be obtained from the external service.</exception>
        Task<Coordinates> GetCoordinatesAsync(string address, Guid correlationId, CancellationToken cancellationToken = default);
    }
}

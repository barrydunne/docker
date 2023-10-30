using Microservices.Shared.Events;

namespace Directions.ExternalService
{
    /// <summary>
    /// Generates directions between two locations.
    /// </summary>
    public interface IExternalService
    {
        /// <summary>
        /// Generates directions between two locations.
        /// </summary>
        /// <param name="startingCoordinates">The starting location for the journey.</param>
        /// <param name="destinationCoordinates">The destination location for the journey.</param>
        /// <param name="correlationId">The correlation id to include in all logging.</param>
        /// <param name="cancellationToken">The token to cancel the operation.</param>
        /// <returns>The coordinates.</returns>
        /// <exception cref="DirectionsException">If directions could not be obtained from the external service.</exception>
        Task<Microservices.Shared.Events.Directions> GetDirectionsAsync(Coordinates startingCoordinates, Coordinates destinationCoordinates, Guid correlationId, CancellationToken cancellationToken = default);
    }
}

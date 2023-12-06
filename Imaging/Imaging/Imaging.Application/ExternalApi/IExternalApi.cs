using Microservices.Shared.Events;

namespace Imaging.Application.ExternalApi;

/// <summary>
/// Gets the URL for an image at a location.
/// </summary>
public interface IExternalApi
{
    /// <summary>
    /// Gets the URL for an image at a location.
    /// </summary>
    /// <param name="address">The target location address for the image.</param>
    /// <param name="coordinates">The target location coordinates for the image.</param>
    /// <param name="correlationId">The correlation id to include in all logging.</param>
    /// <param name="cancellationToken">The token to cancel the operation.</param>
    /// <returns>The coordinates.</returns>
    /// <exception cref="ImagingException">If an image could not be obtained from the external service.</exception>
    Task<string?> GetImageUrlAsync(string address, Coordinates coordinates, Guid correlationId, CancellationToken cancellationToken = default);
}

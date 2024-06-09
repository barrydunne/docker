using Imaging.Application.ExternalApi;
using Microservices.Shared.Events;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Imaging.Infrastructure.ExternalApi.Dummy;

/// <summary>
/// Uses dummy images.
/// </summary>
public class DummyApi : IExternalApi
{
    private readonly ILogger _logger;

    private static readonly ConcurrentDictionary<Coordinates, string> _knownUrls = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DummyApi"/> class.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    public DummyApi(ILogger<DummyApi> logger) => _logger = logger;

    /// <summary>
    /// Add some fixed URLs for specific coordinates.
    /// </summary>
    /// <param name="coordinates">The location of the image.</param>
    /// <param name="url">The known URL to return for these coordinates.</param>
    public static void AddImageUrl(Coordinates coordinates, string url) => _knownUrls[coordinates] = url;

    /// <inheritdoc/>
    public Task<string?> GetImageUrlAsync(string address, Coordinates coordinates, Guid correlationId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Returning image URL. [{CorrelationId}]", correlationId);
        if (!_knownUrls.TryGetValue(coordinates, out var url))
            url = "https://picsum.photos/400/300";
        return Task.FromResult((string?)url);
    }
}

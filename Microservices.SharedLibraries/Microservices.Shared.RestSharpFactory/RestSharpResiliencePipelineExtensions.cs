using RestSharp;

namespace Microservices.Shared.RestSharpFactory;

/// <summary>
/// Provides resilient extension methods for RestSharp.
/// </summary>
public static class RestSharpResiliencePipelineExtensions
{
    /// <summary>
    /// Executes a GET request with retry using the specified <see cref="IRestClient"/> and <see cref="RestRequest"/>.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="client">The <see cref="IRestClient"/> to execute the request.</param>
    /// <param name="request">The <see cref="RestRequest"/> to be executed.</param>
    /// <param name="pipeline">The <see cref="IRestSharpResiliencePipeline"/> to use to wrap the requests.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Deserialized response content.</returns>
    public static async Task<RestResponse<TResponse>> ExecuteGetAsync<TResponse>(this IRestClient client, RestRequest request, IRestSharpResiliencePipeline pipeline, CancellationToken cancellationToken = default)
        => await pipeline.GetPipeline().ExecuteAsync(async _ => await client.ExecuteGetAsync<TResponse>(request, cancellationToken));

    /// <summary>
    /// Executes a POST request with retry using the specified <see cref="IRestClient"/> and <see cref="RestRequest"/>.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="client">The <see cref="IRestClient"/> to execute the request.</param>
    /// <param name="request">The <see cref="RestRequest"/> to be executed.</param>
    /// <param name="pipeline">The <see cref="IRestSharpResiliencePipeline"/> to use to wrap the requests.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Deserialized response content.</returns>
    public static async Task<RestResponse<TResponse>> ExecutePostAsync<TResponse>(this IRestClient client, RestRequest request, IRestSharpResiliencePipeline pipeline, CancellationToken cancellationToken = default)
        => await pipeline.GetPipeline().ExecuteAsync(async _ => await client.ExecuteAsync<TResponse>(request, Method.Post, cancellationToken));
}

using RestSharp;

namespace Microservices.Shared.RestSharpFactory;

/// <summary>
/// A factory for creating <see cref="IRestClient"/> instances.
/// </summary>
public interface IRestSharpClientFactory
{
    /// <summary>
    /// Creates an instance of IRestClient using the provided <see cref="RestClientOptions"/>.
    /// </summary>
    /// <param name="options">Client options.</param>
    /// <param name="configureDefaultHeaders">Delegate to add default headers to the wrapped HttpClient instance.</param>
    /// <param name="configureSerialization">Delegate to configure serialization.</param>
    /// <param name="useClientFactory">Set to true if you wish to reuse the <seealso cref="HttpClient"/> instance.</param>
    /// <returns>An instance of IRestClient.</returns>
    IRestClient CreateRestClient(RestClientOptions options, ConfigureHeaders? configureDefaultHeaders = null, ConfigureSerialization? configureSerialization = null, bool useClientFactory = false);

    /// <summary>
    /// Creates an instance of RestClient using the default <see cref="RestClientOptions"/>.
    /// </summary>
    /// <param name="configureRestClient">Delegate to configure the client options.</param>
    /// <param name="configureDefaultHeaders">Delegate to add default headers to the wrapped HttpClient instance.</param>
    /// <param name="configureSerialization">Delegate to configure serialization.</param>
    /// <param name="useClientFactory">Set to true if you wish to reuse the <seealso cref="HttpClient"/> instance.</param>
    /// <returns>An instance of IRestClient.</returns>
    IRestClient CreateRestClient(ConfigureRestClient? configureRestClient = null, ConfigureHeaders? configureDefaultHeaders = null, ConfigureSerialization? configureSerialization = null, bool useClientFactory = false);

    /// <summary>
    /// Creates an instance of RestClient using a specific BaseUrl for requests made by this client instance.
    /// </summary>
    /// <param name="baseUrl">Base URI for the new client.</param>
    /// <param name="configureRestClient">Delegate to configure the client options.</param>
    /// <param name="configureDefaultHeaders">Delegate to add default headers to the wrapped HttpClient instance.</param>
    /// <param name="configureSerialization">Delegate to configure serialization.</param>
    /// <param name="useClientFactory">Set to true if you wish to reuse the <seealso cref="HttpClient"/> instance.</param>
    /// <returns>An instance of IRestClient.</returns>
    IRestClient CreateRestClient(Uri baseUrl, ConfigureRestClient? configureRestClient = null, ConfigureHeaders? configureDefaultHeaders = null, ConfigureSerialization? configureSerialization = null, bool useClientFactory = false);

    /// <summary>
    /// Creates an instance of RestClient using a specific BaseUrl for requests made by this client instance.
    /// </summary>
    /// <param name="baseUrl">Base URI for this new client as a string.</param>
    /// <param name="configureRestClient">Delegate to configure the client options.</param>
    /// <param name="configureDefaultHeaders">Delegate to add default headers to the wrapped HttpClient instance.</param>
    /// <param name="configureSerialization">Delegate to configure serialization.</param>
    /// <returns>An instance of IRestClient.</returns>
    IRestClient CreateRestClient(string baseUrl, ConfigureRestClient? configureRestClient = null, ConfigureHeaders? configureDefaultHeaders = null, ConfigureSerialization? configureSerialization = null);

    /// <summary>
    /// Creates an instance of RestClient using a shared HttpClient and specific RestClientOptions and does not allocate one internally.
    /// </summary>
    /// <param name="httpClient">HttpClient to use.</param>
    /// <param name="options">RestClient options to use.</param>
    /// <param name="disposeHttpClient">True to dispose of the client, false to assume the caller does (defaults to false).</param>
    /// <param name="configureSerialization">Delegate to configure serialization.</param>
    /// <returns>An instance of IRestClient.</returns>
    IRestClient CreateRestClient(HttpClient httpClient, RestClientOptions? options, bool disposeHttpClient = false, ConfigureSerialization? configureSerialization = null);

    /// <summary>
    /// Creates an instance of RestClient using a shared HttpClient and does not allocate one internally.
    /// </summary>
    /// <param name="httpClient">HttpClient to use.</param>
    /// <param name="disposeHttpClient">True to dispose of the client, false to assume the caller does (defaults to false).</param>
    /// <param name="configureRestClient">Delegate to configure the client options.</param>
    /// <param name="configureSerialization">Delegate to configure serialization.</param>
    /// <returns>An instance of IRestClient.</returns>
    IRestClient CreateRestClient(HttpClient httpClient, bool disposeHttpClient = false, ConfigureRestClient? configureRestClient = null, ConfigureSerialization? configureSerialization = null);

    /// <summary>
    /// Creates a new instance of RestSharp using the message handler provided. By default, HttpClient disposes the provided handler
    /// when the client itself is disposed. If you want to keep the handler not disposed, set disposeHandler argument to false.
    /// </summary>
    /// <param name="handler">Message handler instance to use for HttpClient.</param>
    /// <param name="disposeHandler">Dispose the handler when disposing RestClient, true by default.</param>
    /// <param name="configureRestClient">Delegate to configure the client options.</param>
    /// <param name="configureSerialization">Delegate to configure serialization.</param>
    /// <returns>An instance of IRestClient.</returns>
    IRestClient CreateRestClient(HttpMessageHandler handler, bool disposeHandler = true, ConfigureRestClient? configureRestClient = null, ConfigureSerialization? configureSerialization = null);
}

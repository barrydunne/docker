using IdentityModel.Client;

namespace PublicApi.Api.HttpHandlers
{
    /// <summary>
    /// Provides tokens for use in a development environment. This would not be included in a real application.
    /// </summary>
    public class TokenHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _tokenUrl;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenHandler"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The factory to create HttpClients.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <param name="logger">The logger to write to.</param>
        public TokenHandler(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<TokenHandler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _tokenUrl = $"{configuration.GetConnectionString("ids")}/connect/token";
            _logger = logger;
        }

        /// <summary>
        /// Get an access token to use for Bearer authentication. Obviously this is just for development purposes.
        /// </summary>
        /// <returns>The access token that can be used to call other endpoints.</returns>
        public async Task<string> GetToken()
        {
            using var client = _httpClientFactory.CreateClient();
            _logger.LogDebug("Requesting token from {URL}", _tokenUrl);
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _tokenUrl,
                ClientId = "client",
                ClientSecret = "secret",
                Scope = "publicapi"
            });
            var result = (tokenResponse.IsError ? tokenResponse.Error : tokenResponse.AccessToken) ?? "Failed to get token.";
            _logger.LogDebug("Token Result: {Token}", result);
            return result;
        }
    }
}

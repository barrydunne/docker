using Ardalis.GuardClauses;
using AspNet.KickStarter.CQRS.Abstractions.Queries;
using Microsoft.Extensions.Logging;
using SecretsManager.Logic.Queries;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace SecretsManager.Logic.QueryHandlers
{
    /// <summary>
    /// Handler for the GetSecretsQuery query.
    /// </summary>
    internal class GetSecretsQueryHandler : IQueryHandler<GetSecretsQuery, Dictionary<string, string>>
    {
        private readonly IRedisDatabase _redis;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSecretsQueryHandler"/> class.
        /// </summary>
        /// <param name="redis">The redis database that holds the secrets.</param>
        /// <param name="logger">The logger to write to.</param>
        public GetSecretsQueryHandler(IRedisDatabase redis, ILogger<GetSecretsQueryHandler> logger)
        {
            _redis = redis;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Dictionary<string, string>> Handle(GetSecretsQuery query, CancellationToken cancellationToken)
        {
            Guard.Against.NullOrEmpty(query?.Vault, nameof(GetSecretsQuery.Vault));
            try
            {
                _logger.LogInformation("GetSecretsQuery handler. Vault: {Vault}", query.Vault);
                var secrets = await _redis.GetAsync<Dictionary<string, string>?>(query.Vault.ToSecretVaultName());
                _logger.LogInformation("GetSecretsQuery handler. Found secrets: {Keys}", secrets?.Keys.ToHashSet());
                return secrets ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to find secrets. Vault: {Vault}", query.Vault);
            }
            return new();
        }
    }
}

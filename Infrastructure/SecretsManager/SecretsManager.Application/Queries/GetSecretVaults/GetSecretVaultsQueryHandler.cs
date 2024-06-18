using AspNet.KickStarter.CQRS.Abstractions.Queries;
using AspNet.KickStarter.FunctionalResult;
using Microsoft.Extensions.Logging;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace SecretsManager.Application.Queries.GetSecretVaults;

/// <summary>
/// Handler for the GetSecretVaultsQuery query.
/// </summary>
internal class GetSecretVaultsQueryHandler : IQueryHandler<GetSecretVaultsQuery, string[]>
{
    private readonly IRedisDatabase _redis;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSecretVaultsQueryHandler"/> class.
    /// </summary>
    /// <param name="redis">The redis database that holds the secrets.</param>
    /// <param name="logger">The logger to write to.</param>
    public GetSecretVaultsQueryHandler(IRedisDatabase redis, ILogger<GetSecretVaultsQueryHandler> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<string[]>> Handle(GetSecretVaultsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("GetSecretVaultsQuery query handler.");

            var keys = await _redis.SearchKeysAsync("*".ToSecretVaultName());
            keys = keys.Select(_ => _.FromSecretVaultName());
            _logger.LogInformation("GetSecretVaultsQuery query handler. Found vaults: {Vaults}", keys);
            return keys.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to find vaults.");
        }
        return Array.Empty<string>();
    }
}

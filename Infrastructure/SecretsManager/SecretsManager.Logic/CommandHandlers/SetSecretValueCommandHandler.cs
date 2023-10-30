using Ardalis.GuardClauses;
using AspNet.KickStarter.CQRS.Abstractions.Commands;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using SecretsManager.Logic.Commands;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace SecretsManager.Logic.CommandHandlers
{
    /// <summary>
    /// Handler for the SetSecretValueCommand command.
    /// </summary>
    internal class SetSecretValueCommandHandler : ICommandHandler<SetSecretValueCommand>
    {
        private readonly IRedisDatabase _redis;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetSecretValueCommandHandler"/> class.
        /// </summary>
        /// <param name="redis">The redis database that holds the secrets.</param>
        /// <param name="logger">The logger to write to.</param>
        public SetSecretValueCommandHandler(IRedisDatabase redis, ILogger<SetSecretValueCommandHandler> logger)
        {
            _redis = redis;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Result> Handle(SetSecretValueCommand command, CancellationToken cancellationToken)
        {
            Guard.Against.NullOrEmpty(command?.Vault, nameof(SetSecretValueCommand.Vault));
            Guard.Against.NullOrEmpty(command.Secret, nameof(SetSecretValueCommand.Secret));
            try
            {
                _logger.LogInformation("SetSecretValueCommand handler. Vault: {Vault}, Secret: {Secret}", command.Vault, command.Secret);
                var secrets = await _redis.GetAsync<Dictionary<string, string>>(command.Vault.ToSecretVaultName()) ?? new();
                secrets[command.Secret] = command.Value;
                await _redis.AddAsync(command.Vault.ToSecretVaultName(), secrets);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set secret value. Vault: {Vault}, Secret: {Secret}", command.Vault, command.Secret);
                return Result.Failure(ex.Message);
            }
        }
    }
}

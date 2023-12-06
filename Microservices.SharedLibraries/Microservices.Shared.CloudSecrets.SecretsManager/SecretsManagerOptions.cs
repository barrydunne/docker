using System.Diagnostics.CodeAnalysis;

namespace Microservices.Shared.CloudSecrets.SecretsManager;

/// <summary>
/// The connection options for SecretsManager.
/// </summary>
[ExcludeFromCodeCoverage]
public class SecretsManagerOptions
{
    /// <summary>
    /// Gets or sets the server to connect to, eg http://secretsmanager:8080.
    /// </summary>
    public string BaseUrl { get; set; } = "http://secretsmanager:8080";
}

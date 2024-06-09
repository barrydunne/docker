using System.Diagnostics.CodeAnalysis;

namespace Microservices.Shared.CloudEmail.Aws;

/// <summary>
/// The email options for AWS.
/// </summary>
[ExcludeFromCodeCoverage]
public class AwsEmailOptions
{
    /// <summary>
    /// Gets or sets the sender email address.
    /// </summary>
    public string From { get; set; } = "notifications@microservices.com";
}

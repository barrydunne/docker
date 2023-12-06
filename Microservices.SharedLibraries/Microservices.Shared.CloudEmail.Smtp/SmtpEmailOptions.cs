using System.Diagnostics.CodeAnalysis;

namespace Microservices.Shared.CloudEmail.Smtp;

/// <summary>
/// The connection options for SMTP.
/// Currently only anonymous insecure connections are supported.
/// </summary>
[ExcludeFromCodeCoverage]
public class SmtpEmailOptions
{
    /// <summary>
    /// Gets or sets the sender email address.
    /// </summary>
    public string From { get; set; } = "notifications@microservices.com";

    /// <summary>
    /// Gets or sets the hostname for the SMTP connection.
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Gets or sets the port number for the SMTP connection.
    /// </summary>
    public int Port { get; set; } = 25;
}

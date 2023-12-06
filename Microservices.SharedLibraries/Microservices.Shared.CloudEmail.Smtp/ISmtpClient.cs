using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;

namespace Microservices.Shared.CloudEmail.Smtp;

/// <summary>
/// Provides ability to send SMTP email messages.
/// </summary>
public interface ISmtpClient : IDisposable
{
    /// <summary>
    /// Gets or sets the name or IP address of the host used for SMTP transactions.
    /// </summary>
    [DisallowNull]
    string? Host { get; set; }

    /// <summary>
    /// Gets or sets the port used for SMTP transactions.
    /// </summary>
    int Port { get; set; }

    /// <summary>
    /// Sends the specified message to an SMTP server for delivery as an asynchronous operation.
    /// </summary>
    /// <param name="message">A <see cref="MailMessage"/> that contains the message to send.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SendMailAsync(MailMessage message);
}

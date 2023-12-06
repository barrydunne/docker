using System.Net.Mail;

namespace Microservices.Shared.CloudEmail.Smtp;

/// <summary>
/// An adapter for the <see cref="SmtpClient"/> class to allow mocking.
/// </summary>
public class SmtpClientAdapter : SmtpClient, ISmtpClient { }

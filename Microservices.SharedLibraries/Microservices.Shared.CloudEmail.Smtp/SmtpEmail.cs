﻿using Ardalis.GuardClauses;
using AspNet.KickStarter;
using Microservices.Shared.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net.Mime;

namespace Microservices.Shared.CloudEmail.Smtp;

/// <inheritdoc/>
public class SmtpEmail : ICloudEmail
{
    private readonly SmtpEmailOptions _options;
    private readonly ISmtpClient _smtpClient;
    private readonly ITraceActivity _traceActivity;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmtpEmail"/> class.
    /// </summary>
    /// <param name="options">The connection configuration options.</param>
    /// <param name="smtpClient">The client to use to send emails.</param>
    /// <param name="traceActivity">The trace activity source.</param>
    /// <param name="logger">The logger to write to.</param>
    public SmtpEmail(IOptions<SmtpEmailOptions> options, ISmtpClient smtpClient, ITraceActivity traceActivity, ILogger<SmtpEmail> logger)
    {
        _options = options.Value;
        _smtpClient = smtpClient;
        _smtpClient.Host = _options.Host;
        _smtpClient.Port = _options.Port;
        _traceActivity = traceActivity;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<bool> SendEmailAsync(string subject, string htmlBody, params string[] to)
        => await SendEmailAsync(subject, htmlBody, null, to, null, null);

    /// <inheritdoc/>
    public async Task<bool> SendEmailAsync(string subject, string? htmlBody, string? plainBody, string[] to, string[]? cc, string[]? bcc, params (string Cid, Stream Stream, string ContentType)[] images)
    {
        using var activity = _traceActivity.StartActivity("SMTP Send email");
        Guard.Against.Null(htmlBody ?? plainBody, nameof(htmlBody), $"Must provide either {nameof(htmlBody)} or {nameof(plainBody)}");
        Guard.Against.Empty(to, nameof(to));

        try
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_options.From),
                Subject = subject
            };
            foreach (var recipient in to)
                mail.To.Add(new MailAddress(recipient));
            if (cc is not null)
            {
                foreach (var recipient in cc)
                    mail.CC.Add(new MailAddress(recipient));
            }
            if (bcc is not null)
            {
                foreach (var recipient in bcc)
                    mail.Bcc.Add(new MailAddress(recipient));
            }
            if (!string.IsNullOrWhiteSpace(plainBody))
                mail.Body = plainBody;
            if (!string.IsNullOrWhiteSpace(htmlBody))
            {
                var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                if (images is not null)
                {
                    foreach (var image in images)
                    {
                        var imagelink = new LinkedResource(image.Stream, image.ContentType)
                        {
                            ContentId = image.Cid,
                            TransferEncoding = TransferEncoding.Base64
                        };
                        htmlView.LinkedResources.Add(imagelink);
                    }
                }
                mail.AlternateViews.Add(htmlView);
            }

            _logger.LogInformation("Sending email to {To}", string.Join(", ", to));
            await _smtpClient.SendMailAsync(mail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email");
            return false;
        }
    }
}

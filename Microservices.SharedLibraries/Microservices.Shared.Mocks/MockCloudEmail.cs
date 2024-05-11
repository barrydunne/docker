using Microservices.Shared.CloudEmail;
using System.Collections.Concurrent;

namespace Microservices.Shared.Mocks;

public class MockCloudEmail : ICloudEmail
{
    private bool _sendFailure;
    private Exception? _sendException;

    public ConcurrentBag<(string Subject, string? HtmlBody, string? PlainBody, string[] To, string[]? Cc, string[]? Bcc, (string Cid, Stream Stream, string ContentType)[] Images)> Emails { get; }

    public MockCloudEmail() => Emails = new();

    public Task<bool> SendEmailAsync(string subject, string htmlBody, params string[] to)
         => Task.FromResult(SendEmail(subject, htmlBody, null, to, null, null, Array.Empty<(string Cid, Stream Stream, string ContentType)>()));

    public Task<bool> SendEmailAsync(string subject, string? htmlBody, string? plainBody, string[] to, string[]? cc, string[]? bcc, params (string Cid, Stream Stream, string ContentType)[] images)
        => Task.FromResult(SendEmail(subject, htmlBody, plainBody, to, cc, bcc, images));

    public void WithSendFailure() => _sendFailure = true;

    public void WithSendException(Exception? exception = null) => _sendException = exception ?? new InvalidOperationException();

    private bool SendEmail(string subject, string? htmlBody, string? plainBody, string[] to, string[]? cc, string[]? bcc, params (string Cid, Stream Stream, string ContentType)[] images)
    {
        if (_sendFailure)
            return false;
        if (_sendException is not null)
            throw _sendException;
        Emails.Add((subject, htmlBody, plainBody, to, cc, bcc, images));
        return true;
    }
}

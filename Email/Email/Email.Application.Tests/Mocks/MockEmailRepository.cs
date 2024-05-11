using Email.Application.Models;
using Email.Application.Repositories;
using System.Collections.Concurrent;

namespace Email.Application.Tests.Mocks;

internal class MockEmailRepository : IEmailRepository
{
    private Exception? _getEmailsException;

    internal ConcurrentBag<SentEmail> Emails { get; }

    internal MockEmailRepository() => Emails = new();

    public Task<List<SentEmail>> GetEmailsSentBetweenTimesAsync(DateTimeOffset from, DateTimeOffset to, int skip, int take, CancellationToken cancellationToken = default)
        => Task.FromResult(GetEmails(from, to, skip, take));

    public Task<List<SentEmail>> GetEmailsSentToRecipientAsync(string recipientEmail, int skip, int take, CancellationToken cancellationToken = default)
        => Task.FromResult(GetEmails(recipientEmail, skip, take));

    public Task InsertAsync(SentEmail sentEmail, CancellationToken cancellationToken = default)
    {
        AddEmail(sentEmail);
        return Task.CompletedTask;
    }

    public void WithGetEmailsException(Exception? exception = null) => _getEmailsException = exception ?? new InvalidOperationException();

    private void AddEmail(SentEmail sentEmail) => Emails.Add(sentEmail);

    private List<SentEmail> GetEmails(string recipientEmail, int skip, int take)
        => GetPage(Emails.Where(_ => _.RecipientEmail == recipientEmail), skip, take);

    private List<SentEmail> GetEmails(DateTimeOffset from, DateTimeOffset to, int skip, int take)
        => GetPage(Emails.Where(_ => (_.SentTime >= from) && (_.SentTime <= to)), skip, take);

    private List<SentEmail> GetPage(IEnumerable<SentEmail> emails, int skip, int take)
    {
        if (_getEmailsException is not null)
            throw _getEmailsException;
        return emails.Skip(skip).Take(take).ToList();
    }
}

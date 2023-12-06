using Email.Application.Models;
using Email.Application.Repositories;
using Moq;
using System.Collections.Concurrent;

namespace Email.Application.Tests.Mocks;

internal class MockEmailRepository : Mock<IEmailRepository>
{
    internal ConcurrentBag<SentEmail> Emails { get; }

    internal MockEmailRepository() : base(MockBehavior.Strict)
    {
        Emails = new();

        Setup(_ => _.InsertAsync(It.IsAny<SentEmail>(), It.IsAny<CancellationToken>()))
            .Callback((SentEmail sentEmail, CancellationToken _) => AddEmail(sentEmail))
            .Returns(Task.CompletedTask);

        Setup(_ => _.GetEmailsSentToRecipientAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string recipientEmail, int skip, int take, CancellationToken _) => GetEmails(recipientEmail, skip, take));

        Setup(_ => _.GetEmailsSentBetweenTimesAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DateTimeOffset from, DateTimeOffset to, int skip, int take, CancellationToken _) => GetEmails(from, to, skip, take));
    }

    private void AddEmail(SentEmail sentEmail) => Emails.Add(sentEmail);

    private List<SentEmail> GetEmails(string recipientEmail, int skip, int take)
        => GetPage(Emails.Where(_ => _.RecipientEmail == recipientEmail), skip, take);

    private List<SentEmail> GetEmails(DateTimeOffset from, DateTimeOffset to, int skip, int take)
        => GetPage(Emails.Where(_ => (_.SentTime >= from) && (_.SentTime <= to)), skip, take);

    private static List<SentEmail> GetPage(IEnumerable<SentEmail> emails, int skip, int take)
        => emails.Skip(skip).Take(take).ToList();
}

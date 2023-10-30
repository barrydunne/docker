using Email.Repository;
using Email.Repository.Models;
using Moq;
using System.Collections.Concurrent;

namespace Email.Logic.Tests.Mocks
{
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

            Setup(_ => _.GetEmailsSentBetweenTimesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((DateTime from, DateTime to, int skip, int take, CancellationToken _) => GetEmails(from, to, skip, take));
        }

        private void AddEmail(SentEmail sentEmail) => Emails.Add(sentEmail);

        private List<SentEmail> GetEmails(string recipientEmail, int skip, int take)
            => GetPage(Emails.Where(_ => _.RecipientEmail == recipientEmail), skip, take);

        private List<SentEmail> GetEmails(DateTime from, DateTime to, int skip, int take)
            => GetPage(Emails.Where(_ => (_.SentUtc >= from) && (_.SentUtc <= to)), skip, take);

        private List<SentEmail> GetPage(IEnumerable<SentEmail> emails, int skip, int take)
            => emails.Skip(skip).Take(take).ToList();
    }
}

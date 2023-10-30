using Email.Logic.Queries;
using Email.Repository.Models;
using System.Net.Mail;

namespace Email.Logic.Tests.QueryHandlers.GetEmailsSentBetweenTimesQueryHandler
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "QueryHandlers")]
    internal class GetEmailsSentBetweenTimesQueryHandlerTests
    {
        private readonly Fixture _fixture = new();
        private readonly GetEmailsSentBetweenTimesQueryHandlerTestsContext _context = new();

        [Test]
        public async Task GetEmailsSentBetweenTimesQueryHandler_PerformQueryAsync_returns_data_from_repository()
        {
            var email = _fixture.Create<MailAddress>().Address;
            var data = _fixture.Build<SentEmail>().With(_ => _.RecipientEmail, email).CreateMany();
            _context.WithData(data);
            var query = new GetEmailsSentBetweenTimesQuery(data.Min(_ => _.SentUtc), data.Max(_ => _.SentUtc), data.Count(), 1);
            var results = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(results, Is.EquivalentTo(data));
        }
    }
}

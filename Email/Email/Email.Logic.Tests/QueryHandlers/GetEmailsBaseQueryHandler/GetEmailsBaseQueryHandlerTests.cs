using Email.Logic.Queries;
using Email.Repository.Models;
using System.Net.Mail;

namespace Email.Logic.Tests.QueryHandlers.GetEmailsBaseQueryHandler
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "QueryHandlers")]
    internal class GetEmailsBaseQueryHandlerTests
    {
        private readonly Fixture _fixture = new();
        private readonly GetEmailsBaseQueryHandlerTestsContext _context = new();


        [Test]
        public async Task GetEmailsBaseQueryHandler_metrics_increments_count()
        {
            var email = _fixture.Create<MailAddress>().Address;
            var data = _fixture.Build<SentEmail>().With(_ => _.RecipientEmail, email).CreateMany();
            _context.WithData(data);
            var query = new GetEmailsSentToRecipientQuery(email, data.Count(), 1);
            await _context.Sut.Handle(query, CancellationToken.None);
            _context.AssertMetricsCountIncremented();
        }

        [Test]
        public async Task GetEmailsBaseQueryHandler_metrics_records_load_time()
        {
            var email = _fixture.Create<MailAddress>().Address;
            var data = _fixture.Build<SentEmail>().With(_ => _.RecipientEmail, email).CreateMany();
            _context.WithData(data);
            var query = new GetEmailsSentToRecipientQuery(email, data.Count(), 1);
            await _context.Sut.Handle(query, CancellationToken.None);
            _context.AssertMetricsLoadTimeRecorded();
        }

        [Test]
        public async Task GetEmailsBaseQueryHandler_returns_new_on_exception()
        {
            var email = _fixture.Create<MailAddress>().Address;
            _context.WithException();
            var query = new GetEmailsSentToRecipientQuery(email, 10, 1);
            var result = await _context.Sut.Handle(query, CancellationToken.None);
            Assert.That(result, Is.Empty);
        }
    }
}

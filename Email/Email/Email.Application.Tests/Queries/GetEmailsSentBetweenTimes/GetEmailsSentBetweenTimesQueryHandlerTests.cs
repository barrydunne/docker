using Email.Application.Models;
using Email.Application.Queries.GetEmailsSentBetweenTimes;
using System.Net.Mail;

namespace Email.Application.Tests.Queries.GetEmailsSentBetweenTimes;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Queries")]
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
        var query = new GetEmailsSentBetweenTimesQuery(data.Min(_ => _.SentTime), data.Max(_ => _.SentTime), data.Count(), 1);
        var result = await _context.Sut.Handle(query, CancellationToken.None);
        result.Value.ShouldBe(data, ignoreOrder: true);
    }
}

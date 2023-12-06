using Email.Application.Models;
using Email.Application.Queries.GetEmailsSentToRecipient;
using System.Net.Mail;

namespace Email.Application.Tests.Queries.GetEmailsSentToRecipient;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Queries")]
internal class GetEmailsSentToRecipientQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly GetEmailsSentToRecipientQueryHandlerTestsContext _context = new();

    [Test]
    public async Task GetEmailsSentToRecipientQueryHandler_PerformQueryAsync_returns_data_from_repository()
    {
        var email = _fixture.Create<MailAddress>().Address;
        var data = _fixture.Build<SentEmail>().With(_ => _.RecipientEmail, email).CreateMany();
        _context.WithData(data);
        var query = new GetEmailsSentToRecipientQuery(email, data.Count(), 1);
        var result = await _context.Sut.Handle(query, CancellationToken.None);
        Assert.That(result.Value, Is.EquivalentTo(data));
    }
}

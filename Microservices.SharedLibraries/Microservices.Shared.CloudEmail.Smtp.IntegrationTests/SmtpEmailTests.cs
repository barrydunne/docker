using AutoFixture;
using System.Net.Mail;
using System.Net.Mime;

namespace Microservices.Shared.CloudEmail.Smtp.IntegrationTests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Smtp")]
internal class SmtpEmailTests : IDisposable
{
    private readonly SmtpEmailTestsContext _context = new();
    private readonly Fixture _fixture = new();
    private bool _disposedValue;

    [Test]
    public async Task SendEmailAsync_sends_message_using_images()
    {
        var image = Convert.FromBase64String("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");
        using var stream = new MemoryStream(image);
        var contentId = _fixture.Create<string>();
        var html = $"""Hello <br/> <img src="cid:{contentId}"/> <br/> Goodbye!""";
        var to = new[] { _fixture.Create<MailAddress>().Address };
        var cc = new[] { _fixture.Create<MailAddress>().Address, _fixture.Create<MailAddress>().Address };
        var subject = _fixture.Create<string>();
        var result = await _context.Sut.SendEmailAsync(subject, html, null, to, cc, null, (contentId, stream, MediaTypeNames.Image.Png));
        Assert.That(result, Is.True);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
                _context.Dispose();
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

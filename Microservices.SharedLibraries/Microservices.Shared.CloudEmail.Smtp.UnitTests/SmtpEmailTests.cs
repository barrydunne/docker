using NUnit.Framework.Constraints;
using System.Net.Mail;
using System.Net.Mime;

namespace Microservices.Shared.CloudEmail.Smtp.UnitTests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Smtp")]
internal class SmtpEmailTests
{
    private readonly SmtpEmailTestsContext _context = new();
    private readonly Fixture _fixture = new();

    [Test]
    public void Constructor_sets_host_from_options()
    {
        var _ = _context.Sut;
        _context.AssertHostSet();
    }

    [Test]
    public void Constructor_sets_port_from_options()
    {
        var _ = _context.Sut;
        _context.AssertPortSet();
    }

    [Test]
    public void SendEmailAsync_guards_against_missing_body()
        => Assert.That(async () => await _context.Sut.SendEmailAsync(_fixture.Create<string>(), null!, new[] { _fixture.Create<MailAddress>().Address }), 
            Throws.TypeOf<ArgumentNullException>()
            .With.Property("Message").EqualTo("Must provide either htmlBody or plainBody (Parameter 'htmlBody')"));

    [Test]
    public void SendEmailAsync_guards_against_missing_to()
        => Assert.That(async () => await _context.Sut.SendEmailAsync(_fixture.Create<string>(), _fixture.Create<string>(), Array.Empty<string>()),
            Throws.TypeOf<ArgumentException>()
            .With.Property("Message").EqualTo("Required input to was empty. (Parameter 'to')"));

    [Test]
    public async Task SendEmailAsync_sends_message_using_subject()
    {
        var (subject, htmlBody, plainBody, to, cc, bcc) = CreateAnonymousValues();
        await _context.Sut.SendEmailAsync(subject, htmlBody, plainBody, to, cc, bcc);
        _context.AssertMessageSent(message => message.Subject == subject);
    }

    [Test]
    public async Task SendEmailAsync_sends_message_using_html_body()
    {
        var (subject, htmlBody, plainBody, to, cc, bcc) = CreateAnonymousValues();
        await _context.Sut.SendEmailAsync(subject, htmlBody, plainBody, to, cc, bcc);
        _context.AssertMessageSent(message => message.AlternateViews.FirstOrDefault(_ => _.ContentType.ToString().Contains(MediaTypeNames.Text.Html)) is not null);
    }

    [Test]
    public async Task SendEmailAsync_sends_message_using_plain_body()
    {
        var (subject, htmlBody, plainBody, to, cc, bcc) = CreateAnonymousValues();
        await _context.Sut.SendEmailAsync(subject, htmlBody, plainBody, to, cc, bcc);
        _context.AssertMessageSent(message => message.Body == plainBody);
    }

    [Test]
    public async Task SendEmailAsync_sends_message_using_to()
    {
        var (subject, htmlBody, plainBody, to, cc, bcc) = CreateAnonymousValues();
        await _context.Sut.SendEmailAsync(subject, htmlBody, plainBody, to, cc, bcc);
        _context.AssertMessageSent(message => new CollectionEquivalentConstraint(to).ApplyTo(message.To.Select(_ => _.Address)).Status == ConstraintStatus.Success);
    }

    [Test]
    public async Task SendEmailAsync_sends_message_using_cc()
    {
        var (subject, htmlBody, plainBody, to, cc, bcc) = CreateAnonymousValues();
        await _context.Sut.SendEmailAsync(subject, htmlBody, plainBody, to, cc, bcc);
        _context.AssertMessageSent(message => new CollectionEquivalentConstraint(cc).ApplyTo(message.CC.Select(_ => _.Address)).Status == ConstraintStatus.Success);
    }

    [Test]
    public async Task SendEmailAsync_sends_message_using_bcc()
    {
        var (subject, htmlBody, plainBody, to, cc, bcc) = CreateAnonymousValues();
        await _context.Sut.SendEmailAsync(subject, htmlBody, plainBody, to, cc, bcc);
        _context.AssertMessageSent(message => new CollectionEquivalentConstraint(bcc).ApplyTo(message.Bcc.Select(_ => _.Address)).Status == ConstraintStatus.Success);
    }

    [Test]
    public async Task SendEmailAsync_sends_message_using_images()
    {
        var (subject, htmlBody, plainBody, to, cc, bcc) = CreateAnonymousValues();
        var image1Cid = _fixture.Create<string>();
        using var image1Stream = new MemoryStream();
        var image1ContentType = MediaTypeNames.Image.Jpeg;
        var image2Cid = _fixture.Create<string>();
        using var image2Stream = new MemoryStream();
        var image2ContentType = MediaTypeNames.Image.Gif;
        await _context.Sut.SendEmailAsync(subject, htmlBody, plainBody, to, cc, bcc, new(image1Cid, image1Stream, image1ContentType), new(image2Cid, image2Stream, image2ContentType));
        _context.AssertMessageSent(message => 
        {
            var htmlView = message.AlternateViews.FirstOrDefault(_ => _.ContentType.ToString().Contains(MediaTypeNames.Text.Html));
            if (htmlView is null)
                return false;
            if (htmlView.LinkedResources.Count != 2)
                return false;
            var resource1 = htmlView.LinkedResources[0];
            if ((resource1.ContentId != image1Cid) || (resource1.ContentStream != image1Stream) || (!resource1.ContentType.ToString().Contains(image1ContentType)))
                return false;
            var resource2 = htmlView.LinkedResources[1];
            if ((resource2.ContentId != image2Cid) || (resource2.ContentStream != image2Stream) || (!resource2.ContentType.ToString().Contains(image2ContentType)))
                return false;
            return true;
        });
    }

    [Test]
    public async Task SendEmailAsync_returns_true_on_success()
    {
        var (subject, htmlBody, plainBody, to, cc, bcc) = CreateAnonymousValues();
        var result = await _context.Sut.SendEmailAsync(subject, htmlBody, plainBody, to, cc, bcc);
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task SendEmailAsync_returns_false_on_exception()
    {
        _context.WithSendException();
        var (subject, htmlBody, plainBody, to, cc, bcc) = CreateAnonymousValues();
        var result = await _context.Sut.SendEmailAsync(subject, htmlBody, plainBody, to, cc, bcc);
        Assert.That(result, Is.False);
    }

    private (string subject, string htmlBody, string plainBody, string[] to, string[] cc, string[] bcc) CreateAnonymousValues()
        => (
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.CreateMany<MailAddress>().Select(_ => _.Address).ToArray(),
            _fixture.CreateMany<MailAddress>().Select(_ => _.Address).ToArray(),
            _fixture.CreateMany<MailAddress>().Select(_ => _.Address).ToArray()
        );
}

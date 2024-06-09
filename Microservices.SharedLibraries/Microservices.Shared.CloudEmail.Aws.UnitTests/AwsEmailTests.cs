using NUnit.Framework.Constraints;
using System.Net.Mail;
using System.Net.Mime;

namespace Microservices.Shared.CloudEmail.Aws.UnitTests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "AwsEmail")]
internal class AwsEmailTests
{
    private readonly AwsEmailTestsContext _context = new();
    private readonly Fixture _fixture = new();

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
        _context.AssertMessageSent(message => message.HtmlBody == htmlBody);
    }

    [Test]
    public async Task SendEmailAsync_sends_message_using_plain_body()
    {
        var (subject, htmlBody, plainBody, to, cc, bcc) = CreateAnonymousValues();
        await _context.Sut.SendEmailAsync(subject, htmlBody, plainBody, to, cc, bcc);
        _context.AssertMessageSent(message => message.TextBody == plainBody);
    }

    [Test]
    public async Task SendEmailAsync_sends_message_using_to()
    {
        var (subject, htmlBody, plainBody, to, cc, bcc) = CreateAnonymousValues();
        await _context.Sut.SendEmailAsync(subject, htmlBody, plainBody, to, cc, bcc);
        _context.AssertMessageSent(message => new CollectionEquivalentConstraint(to).ApplyTo(message.To.Select(_ => _.ToString())).Status == ConstraintStatus.Success);
    }

    [Test]
    public async Task SendEmailAsync_sends_message_using_cc()
    {
        var (subject, htmlBody, plainBody, to, cc, bcc) = CreateAnonymousValues();
        await _context.Sut.SendEmailAsync(subject, htmlBody, plainBody, to, cc, bcc);
        _context.AssertMessageSent(message => new CollectionEquivalentConstraint(cc).ApplyTo(message.Cc.Select(_ => _.ToString())).Status == ConstraintStatus.Success);
    }

    [Test]
    public async Task SendEmailAsync_sends_message_using_bcc()
    {
        var (subject, htmlBody, plainBody, to, cc, bcc) = CreateAnonymousValues();
        await _context.Sut.SendEmailAsync(subject, htmlBody, plainBody, to, cc, bcc);
        _context.AssertMessageSent(message => new CollectionEquivalentConstraint(bcc).ApplyTo(message.Bcc.Select(_ => _.ToString())).Status == ConstraintStatus.Success);
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
            var htmlView = message.BodyParts.FirstOrDefault(_ => _.ContentType.ToString().Contains(MediaTypeNames.Text.Html));
            if (htmlView is null)
                return false;
            var image1 = message.BodyParts.FirstOrDefault(_ => _.ContentId == image1Cid);
            if (image1 is null)
                return false;
            var image2 = message.BodyParts.FirstOrDefault(_ => _.ContentId == image2Cid);
            if (image2 is null)
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

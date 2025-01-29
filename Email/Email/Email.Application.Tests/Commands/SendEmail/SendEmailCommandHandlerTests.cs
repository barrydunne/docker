using Email.Application.Commands.SendEmail;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;

namespace Email.Application.Tests.Commands.SendEmail;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class SendEmailCommandHandlerTests
{
    private readonly Fixture _fixture;
    private readonly SendEmailCommandHandlerTestsContext _context;

    public SendEmailCommandHandlerTests()
    {
        _fixture = new();
        _fixture.Customizations.Add(new MicroserviceSpecimenBuilder());
        _context = new();
    }

    [Test]
    public async Task SendEmailCommandHandler_metrics_increments_count()
    {
        var command = _fixture.Create<SendEmailCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsCountIncremented();
    }

    [Test]
    public async Task SendEmailCommandHandler_metrics_records_image_time()
    {
        var command = _fixture.Create<SendEmailCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsImageTimeRecorded();
    }

    [Test]
    public async Task SendEmailCommandHandler_metrics_records_generate_time()
    {
        var command = _fixture.Create<SendEmailCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsGenerateTimeRecorded();
    }

    [Test]
    public async Task SendEmailCommandHandler_metrics_records_email_time()
    {
        var command = _fixture.Create<SendEmailCommand>();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertMetricsEmailTimeRecorded();
    }

    [Test]
    public async Task SendEmailCommandHandler_generates_html_without_image_if_unavailable()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.Imaging, _fixture.Build<ImagingResult>().With(_ => _.IsSuccessful, true).Create())
                              .Create();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertHtmlGeneratedWithoutImage();
    }

    [Test]
    public async Task SendEmailCommandHandler_generates_html_with_image_if_available()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.Imaging, _fixture.Build<ImagingResult>().With(_ => _.IsSuccessful, true).Create())
                              .Create();
        _context.WithImage(command.Imaging!.ImagePath!);
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertHtmlGeneratedWithImage();
    }

    [Test]
    public async Task SendEmailCommandHandler_sends_email_without_image_if_unavailable()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.Imaging, _fixture.Build<ImagingResult>().With(_ => _.IsSuccessful, true).Create())
                              .Create();
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertEmailSentWithoutImage();
    }

    [Test]
    public async Task SendEmailCommandHandler_sends_email_with_image_if_available()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.Imaging, _fixture.Build<ImagingResult>().With(_ => _.IsSuccessful, true).Create())
                              .Create();
        _context.WithImage(command.Imaging!.ImagePath!);
        await _context.Sut.Handle(command, CancellationToken.None);
        _context.AssertEmailSentWithImage();
    }

    [Test]
    public async Task SendEmailCommandHandler_returns_error_if_fail_to_send_email()
    {
        var command = _fixture.Create<SendEmailCommand>();
        _context.WithSendFailure();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.IsError.ShouldBeTrue();
    }

    [Test]
    public async Task SendEmailCommandHandler_returns_error_on_exception()
    {
        var command = _fixture.Create<SendEmailCommand>();
        _context.WithSendException();
        var result = await _context.Sut.Handle(command, CancellationToken.None);
        result.IsError.ShouldBeTrue();
    }
}

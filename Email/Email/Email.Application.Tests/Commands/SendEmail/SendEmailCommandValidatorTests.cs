using Email.Application.Commands.SendEmail;
using FluentValidation.TestHelper;
using Microservices.Shared.Events;

namespace Email.Application.Tests.Commands.SendEmail;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class SendEmailCommandValidatorTests
{
    private readonly EmailFixture _fixture = new();
    private readonly SendEmailCommandValidatorTestsContext _context = new();

    [Test]
    public async Task SendEmailCommandValidator_metrics_records_guard_time()
    {
        var command = _fixture.Create<SendEmailCommand>();
        await _context.Sut.TestValidateAsync(command);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task SendEmailCommandValidator_succeeds_for_valid_instance()
    {
        var command = _fixture.Create<SendEmailCommand>();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task SendEmailCommandValidator_fails_for_missing_job_id()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task SendEmailCommandValidator_returns_message_for_missing_job_id()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.JobId) && _.ErrorMessage == "'Job Id' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task SendEmailCommandValidator_fails_for_missing_email()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.Email, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task SendEmailCommandValidator_returns_message_for_missing_email()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.Email, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.Email) && _.ErrorMessage == "'Email' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task SendEmailCommandValidator_fails_for_invalid_email()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.Email, "invalid")
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task SendEmailCommandValidator_returns_message_for_invalid_email()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.Email, "invalid")
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.Email) && _.ErrorMessage == "'Email' is not a valid email address.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task SendEmailCommandValidator_fails_for_missing_starting_address()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.StartingAddress, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task SendEmailCommandValidator_returns_message_for_missing_starting_address()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.StartingAddress, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.StartingAddress) && _.ErrorMessage == "'Starting Address' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task SendEmailCommandValidator_fails_for_missing_destination_address()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.DestinationAddress, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task SendEmailCommandValidator_returns_message_for_missing_destination_address()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.DestinationAddress, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.DestinationAddress) && _.ErrorMessage == "'Destination Address' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task SendEmailCommandValidator_fails_for_missing_directions()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.Directions, (Directions)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task SendEmailCommandValidator_returns_message_for_missing_directions()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.Directions, (Directions)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.Directions) && _.ErrorMessage == "'Directions' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task SendEmailCommandValidator_fails_for_missing_weather()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.Weather, (WeatherForecast)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task SendEmailCommandValidator_returns_message_for_missing_weather()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.Weather, (WeatherForecast)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.Weather) && _.ErrorMessage == "'Weather' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task SendEmailCommandValidator_fails_for_missing_imaging()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.Imaging, (ImagingResult)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task SendEmailCommandValidator_returns_message_for_missing_imaging()
    {
        var command = _fixture.Build<SendEmailCommand>()
                              .With(_ => _.Imaging, (ImagingResult)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.Imaging) && _.ErrorMessage == "'Imaging' must not be empty.");
        error.ShouldNotBeNull();
    }
}

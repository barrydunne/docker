using FluentValidation.TestHelper;
using State.Application.Commands.CreateJob;

namespace State.Application.Tests.Commands.CreateJob;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class CreateJobCommandValidatorTests
{
    private readonly StateFixture _fixture = new();
    private readonly CreateJobCommandValidatorTestsContext _context = new();

    [Test]
    public async Task CreateJobCommandValidator_metrics_records_guard_time()
    {
        var command = _fixture.Create<CreateJobCommand>();
        await _context.Sut.TestValidateAsync(command);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task CreateJobCommandValidator_succeeds_for_valid_instance()
    {
        var command = _fixture.Create<CreateJobCommand>();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public async Task CreateJobCommandValidator_fails_for_missing_job_id()
    {
        var command = _fixture.Build<CreateJobCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task CreateJobCommandValidator_returns_message_for_missing_job_id()
    {
        var command = _fixture.Build<CreateJobCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.JobId) && _.ErrorMessage == "'Job Id' must not be empty.");
        Assert.That(error, Is.Not.Null);
    }

    [Test]
    public async Task CreateJobCommandValidator_fails_for_missing_starting_address()
    {
        var command = _fixture.Build<CreateJobCommand>()
                              .With(_ => _.StartingAddress, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task CreateJobCommandValidator_returns_message_for_missing_starting_address()
    {
        var command = _fixture.Build<CreateJobCommand>()
                              .With(_ => _.StartingAddress, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.StartingAddress) && _.ErrorMessage == "'Starting Address' must not be empty.");
        Assert.That(error, Is.Not.Null);
    }

    [Test]
    public async Task CreateJobCommandValidator_fails_for_missing_destination_address()
    {
        var command = _fixture.Build<CreateJobCommand>()
                              .With(_ => _.DestinationAddress, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task CreateJobCommandValidator_returns_message_for_missing_destination_address()
    {
        var command = _fixture.Build<CreateJobCommand>()
                              .With(_ => _.DestinationAddress, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.DestinationAddress) && _.ErrorMessage == "'Destination Address' must not be empty.");
        Assert.That(error, Is.Not.Null);
    }

    [Test]
    public async Task CreateJobCommandValidator_fails_for_missing_email()
    {
        var command = _fixture.Build<CreateJobCommand>()
                              .With(_ => _.Email, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task CreateJobCommandValidator_returns_message_for_missing_email()
    {
        var command = _fixture.Build<CreateJobCommand>()
                              .With(_ => _.Email, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.Email) && _.ErrorMessage == "'Email' must not be empty.");
        Assert.That(error, Is.Not.Null);
    }
}

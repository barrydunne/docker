using Directions.Application.Commands.GenerateDirections;
using FluentValidation.TestHelper;

namespace Directions.Application.Tests.CommandHandlers.GenerateDirectionsCommandHandler;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class GenerateDirectionsCommandValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly GenerateDirectionsCommandValidatorTestsContext _context = new();

    [Test]
    public async Task GenerateDirectionsCommandValidator_metrics_records_guard_time()
    {
        var command = _fixture.Create<GenerateDirectionsCommand>();
        await _context.Sut.TestValidateAsync(command);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task GenerateDirectionsCommandValidator_succeeds_for_valid_instance()
    {
        var command = _fixture.Create<GenerateDirectionsCommand>();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public async Task GenerateDirectionsCommandValidator_fails_for_missing_job_id()
    {
        var command = _fixture.Build<GenerateDirectionsCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task GenerateDirectionsCommandValidator_returns_message_for_missing_job_id()
    {
        var command = _fixture.Build<GenerateDirectionsCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.JobId) && _.ErrorMessage == "'Job Id' must not be empty.");
        Assert.That(error, Is.Not.Null);
    }
}

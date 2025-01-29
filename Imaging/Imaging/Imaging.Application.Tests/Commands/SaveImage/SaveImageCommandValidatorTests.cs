using FluentValidation.TestHelper;
using Imaging.Application.Commands.SaveImage;
using Microservices.Shared.Events;

namespace Imaging.Application.Tests.Commands.SaveImage;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class SaveImageCommandValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly SaveImageCommandValidatorTestsContext _context = new();

    [Test]
    public async Task SaveImageCommandValidator_metrics_records_guard_time()
    {
        var command = _fixture.Create<SaveImageCommand>();
        await _context.Sut.TestValidateAsync(command);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task SaveImageCommandValidator_succeeds_for_valid_instance()
    {
        var command = _fixture.Create<SaveImageCommand>();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task SaveImageCommandValidator_fails_for_missing_job_id()
    {
        var command = _fixture.Build<SaveImageCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task SaveImageCommandValidator_returns_message_for_missing_job_id()
    {
        var command = _fixture.Build<SaveImageCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.JobId) && _.ErrorMessage == "'Job Id' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task SaveImageCommandValidator_fails_for_missing_address()
    {
        var command = _fixture.Build<SaveImageCommand>()
                              .With(_ => _.Address, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task SaveImageCommandValidator_returns_message_for_missing_address()
    {
        var command = _fixture.Build<SaveImageCommand>()
                              .With(_ => _.Address, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.Address) && _.ErrorMessage == "'Address' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task SaveImageCommandValidator_fails_for_missing_coordinates()
    {
        var command = _fixture.Build<SaveImageCommand>()
                              .With(_ => _.Coordinates, (Coordinates)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task SaveImageCommandValidator_returns_message_for_missing_coordinates()
    {
        var command = _fixture.Build<SaveImageCommand>()
                              .With(_ => _.Coordinates, (Coordinates)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.Coordinates) && _.ErrorMessage == "'Coordinates' must not be empty.");
        error.ShouldNotBeNull();
    }
}

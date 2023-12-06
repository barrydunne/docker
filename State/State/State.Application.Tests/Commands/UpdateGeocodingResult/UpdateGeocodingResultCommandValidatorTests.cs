using FluentValidation.TestHelper;
using Microservices.Shared.Events;
using State.Application.Commands.UpdateGeocodingResult;

namespace State.Application.Tests.Commands.UpdateGeocodingResult;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class UpdateGeocodingResultCommandValidatorTests
{
    private readonly StateFixture _fixture = new();
    private readonly UpdateGeocodingResultCommandValidatorTestsContext _context = new();

    [Test]
    public async Task UpdateGeocodingResultCommandValidator_metrics_records_guard_time()
    {
        var command = _fixture.Create<UpdateGeocodingResultCommand>();
        await _context.Sut.TestValidateAsync(command);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task UpdateGeocodingResultCommandValidator_succeeds_for_valid_instance()
    {
        var command = _fixture.Create<UpdateGeocodingResultCommand>();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public async Task UpdateGeocodingResultCommandValidator_fails_for_missing_job_id()
    {
        var command = _fixture.Build<UpdateGeocodingResultCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task UpdateGeocodingResultCommandValidator_returns_message_for_missing_job_id()
    {
        var command = _fixture.Build<UpdateGeocodingResultCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.JobId) && _.ErrorMessage == "'Job Id' must not be empty.");
        Assert.That(error, Is.Not.Null);
    }

    [Test]
    public async Task UpdateGeocodingResultCommandValidator_fails_for_missing_starting_coordinates()
    {
        var command = _fixture.Build<UpdateGeocodingResultCommand>()
                              .With(_ => _.StartingCoordinates, (GeocodingCoordinates)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task UpdateGeocodingResultCommandValidator_returns_message_for_missing_starting_coordinates()
    {
        var command = _fixture.Build<UpdateGeocodingResultCommand>()
                              .With(_ => _.StartingCoordinates, (GeocodingCoordinates)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.StartingCoordinates) && _.ErrorMessage == "'Starting Coordinates' must not be empty.");
        Assert.That(error, Is.Not.Null);
    }

    [Test]
    public async Task UpdateGeocodingResultCommandValidator_fails_for_missing_destination_coordinates()
    {
        var command = _fixture.Build<UpdateGeocodingResultCommand>()
                              .With(_ => _.DestinationCoordinates, (GeocodingCoordinates)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        Assert.That(result.IsValid, Is.False);
    }

    [Test]
    public async Task UpdateGeocodingResultCommandValidator_returns_message_for_missing_destination_coordinates()
    {
        var command = _fixture.Build<UpdateGeocodingResultCommand>()
                              .With(_ => _.DestinationCoordinates, (GeocodingCoordinates)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.DestinationCoordinates) && _.ErrorMessage == "'Destination Coordinates' must not be empty.");
        Assert.That(error, Is.Not.Null);
    }
}

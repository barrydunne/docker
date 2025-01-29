using Directions.Application.Queries.GetDirections;
using FluentValidation.TestHelper;
using Microservices.Shared.Events;

namespace Directions.Application.Tests.QueryHandlers.GetDirectionsQueryHandler;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Queries")]
internal class GetDirectionsQueryValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly GetDirectionsQueryValidatorTestsContext _context = new();

    [Test]
    public async Task GetDirectionsQueryValidator_metrics_records_guard_time()
    {
        var command = _fixture.Create<GetDirectionsQuery>();
        await _context.Sut.TestValidateAsync(command);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task GetDirectionsQueryValidator_succeeds_for_valid_instance()
    {
        var command = _fixture.Create<GetDirectionsQuery>();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task GetDirectionsQueryValidator_fails_for_missing_job_id()
    {
        var command = _fixture.Build<GetDirectionsQuery>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetDirectionsQueryValidator_returns_message_for_missing_job_id()
    {
        var command = _fixture.Build<GetDirectionsQuery>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.JobId) && _.ErrorMessage == "'Job Id' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GetDirectionsQueryValidator_fails_for_missing_starting_coordinates()
    {
        var command = _fixture.Build<GetDirectionsQuery>()
                              .With(_ => _.StartingCoordinates, (Coordinates)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetDirectionsQueryValidator_returns_message_for_missing_starting_coordinates()
    {
        var command = _fixture.Build<GetDirectionsQuery>()
                              .With(_ => _.StartingCoordinates, (Coordinates)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.StartingCoordinates) && _.ErrorMessage == "'Starting Coordinates' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GetDirectionsQueryValidator_fails_for_missing_destination_coordinates()
    {
        var command = _fixture.Build<GetDirectionsQuery>()
                              .With(_ => _.DestinationCoordinates, (Coordinates)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetDirectionsQueryValidator_returns_message_for_missing_destination_coordinates()
    {
        var command = _fixture.Build<GetDirectionsQuery>()
                              .With(_ => _.DestinationCoordinates, (Coordinates)null!)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.DestinationCoordinates) && _.ErrorMessage == "'Destination Coordinates' must not be empty.");
        error.ShouldNotBeNull();
    }
}

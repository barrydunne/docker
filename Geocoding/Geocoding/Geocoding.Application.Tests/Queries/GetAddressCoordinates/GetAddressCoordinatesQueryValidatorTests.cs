using FluentValidation.TestHelper;
using Geocoding.Application.Queries.GetAddressCoordinates;

namespace Geocoding.Application.Tests.Queries.GetAddressCoordinates;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Queries")]
internal class GetAddressCoordinatesQueryValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly GetAddressCoordinatesQueryValidatorTestsContext _context = new();

    [Test]
    public async Task GetAddressCoordinatesQueryValidator_metrics_records_guard_time()
    {
        var command = _fixture.Create<GetAddressCoordinatesQuery>();
        await _context.Sut.TestValidateAsync(command);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task GetAddressCoordinatesQueryValidator_succeeds_for_valid_instance()
    {
        var command = _fixture.Create<GetAddressCoordinatesQuery>();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task GetAddressCoordinatesQueryValidator_fails_for_missing_job_id()
    {
        var command = _fixture.Build<GetAddressCoordinatesQuery>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetAddressCoordinatesQueryValidator_returns_message_for_missing_job_id()
    {
        var command = _fixture.Build<GetAddressCoordinatesQuery>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.JobId) && _.ErrorMessage == "'Job Id' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GetAddressCoordinatesQueryValidator_fails_for_missing_address()
    {
        var command = _fixture.Build<GetAddressCoordinatesQuery>()
                              .With(_ => _.Address, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GetAddressCoordinatesQueryValidator_returns_message_for_missing_address()
    {
        var command = _fixture.Build<GetAddressCoordinatesQuery>()
                              .With(_ => _.Address, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.Address) && _.ErrorMessage == "'Address' must not be empty.");
        error.ShouldNotBeNull();
    }
}

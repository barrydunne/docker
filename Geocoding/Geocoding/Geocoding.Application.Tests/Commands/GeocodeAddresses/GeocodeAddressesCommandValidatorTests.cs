using FluentValidation.TestHelper;
using Geocoding.Application.Commands.GeocodeAddresses;

namespace Geocoding.Application.Tests.Commands.GeocodeAddresses;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Commands")]
internal class GeocodeAddressesCommandValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly GeocodeAddressesCommandValidatorTestsContext _context = new();

    [Test]
    public async Task GeocodeAddressesCommandValidator_metrics_records_guard_time()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        await _context.Sut.TestValidateAsync(command);
        _context.AssertMetricsGuardTimeRecorded();
    }

    [Test]
    public async Task GeocodeAddressesCommandValidator_succeeds_for_valid_instance()
    {
        var command = _fixture.Create<GeocodeAddressesCommand>();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeTrue();
    }

    [Test]
    public async Task GeocodeAddressesCommandValidator_fails_for_missing_job_id()
    {
        var command = _fixture.Build<GeocodeAddressesCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GeocodeAddressesCommandValidator_returns_message_for_missing_job_id()
    {
        var command = _fixture.Build<GeocodeAddressesCommand>()
                              .With(_ => _.JobId, Guid.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.JobId) && _.ErrorMessage == "'Job Id' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GeocodeAddressesCommandValidator_fails_for_missing_starting_address()
    {
        var command = _fixture.Build<GeocodeAddressesCommand>()
                              .With(_ => _.StartingAddress, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GeocodeAddressesCommandValidator_returns_message_for_missing_starting_address()
    {
        var command = _fixture.Build<GeocodeAddressesCommand>()
                              .With(_ => _.StartingAddress, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.StartingAddress) && _.ErrorMessage == "'Starting Address' must not be empty.");
        error.ShouldNotBeNull();
    }

    [Test]
    public async Task GeocodeAddressesCommandValidator_fails_for_missing_destination_address()
    {
        var command = _fixture.Build<GeocodeAddressesCommand>()
                              .With(_ => _.DestinationAddress, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        result.IsValid.ShouldBeFalse();
    }

    [Test]
    public async Task GeocodeAddressesCommandValidator_returns_message_for_missing_destination_address()
    {
        var command = _fixture.Build<GeocodeAddressesCommand>()
                              .With(_ => _.DestinationAddress, string.Empty)
                              .Create();
        var result = await _context.Sut.TestValidateAsync(command);
        var error = result.Errors.SingleOrDefault(_ => _.PropertyName == nameof(command.DestinationAddress) && _.ErrorMessage == "'Destination Address' must not be empty.");
        error.ShouldNotBeNull();
    }
}

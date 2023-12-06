using Weather.Application.Queries.GetWeather;

namespace Weather.Application.Tests.Queries.GetWeather;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Queries")]
internal class GetWeatherQueryHandlerTests
{
    private readonly Fixture _fixture = new();
    private readonly GetWeatherQueryHandlerTestsContext _context = new();

    [Test]
    public async Task GeocodeAddressesCommandHandler_metrics_increments_count()
    {
        var query = _fixture.Create<GetWeatherQuery>();
        await _context.Sut.Handle(query, CancellationToken.None);
        _context.AssertMetricsCountIncremented();
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_metrics_records_external_time()
    {
        var query = _fixture.Create<GetWeatherQuery>();
        await _context.Sut.Handle(query, CancellationToken.None);
        _context.AssertMetricsExternalTimeRecorded();
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_returns_result_from_external()
    {
        var query = _fixture.Create<GetWeatherQuery>();
        var weather = _context.CreateWeatherForecast();
        _context.WithExternalResult(weather);
        var result = await _context.Sut.Handle(query, CancellationToken.None);
        Assert.That(result.Value, Is.EqualTo(weather));
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_returns_error_on_exception()
    {
        var query = _fixture.Create<GetWeatherQuery>();
        var message = _fixture.Create<string>();
        _context.WithException(message);
        var result = await _context.Sut.Handle(query, CancellationToken.None);
        Assert.That(result.IsError, Is.True);
    }

    [Test]
    public async Task GeocodeAddressesCommandHandler_returns_message_on_exception()
    {
        var query = _fixture.Create<GetWeatherQuery>();
        var message = _fixture.Create<string>();
        _context.WithException(message);
        var result = await _context.Sut.Handle(query, CancellationToken.None);
        Assert.That(result.Error?.Message, Is.EqualTo(message));
    }
}

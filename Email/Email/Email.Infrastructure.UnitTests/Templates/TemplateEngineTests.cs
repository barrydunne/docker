using Email.Infrastructure.Templates;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;

namespace Email.Infrastructure.UnitTests.Templates;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.Self)]
[TestFixture(Category = "Templates")]
internal class TemplateEngineTests
{
    private readonly Fixture _fixture;

    public TemplateEngineTests()
    {
        _fixture = new();
        _fixture.Customizations.Add(new MicroserviceSpecimenBuilder());
    }

    [Test]
    public void GenerateHtml_includes_starting_address()
    {
        (var startingAddress, var destinationAddress, var directions, var weather, var imageCid) = CreateInput();
        var html = new TemplateEngine().GenerateHtml(startingAddress, destinationAddress, directions, weather, imageCid);
        Assert.That(html, Does.Contain(startingAddress));
    }

    [Test]
    public void GenerateHtml_includes_destination_address()
    {
        (var startingAddress, var destinationAddress, var directions, var weather, var imageCid) = CreateInput();
        var html = new TemplateEngine().GenerateHtml(startingAddress, destinationAddress, directions, weather, imageCid);
        Assert.That(html, Does.Contain(destinationAddress));
    }

    [Test]
    public void GenerateHtml_includes_directions_if_available()
    {
        (var startingAddress, var destinationAddress, var directions, var weather, var imageCid) = CreateInput();
        var html = new TemplateEngine().GenerateHtml(startingAddress, destinationAddress, directions, weather, imageCid);
        Assert.That(html, Does.Contain(directions.Steps![0].Description));
        Assert.That(html, Does.Not.Contain("No Directions Available"));
    }

    [Test]
    public void GenerateHtml_includes_no_directions_if_not_successful()
    {
        (var startingAddress, var destinationAddress, var directions, var weather, var imageCid) = CreateInput(withDirectionsSuccessful: false);
        var html = new TemplateEngine().GenerateHtml(startingAddress, destinationAddress, directions, weather, imageCid);
        Assert.That(html, Does.Contain("No Directions Available"));
        Assert.That(html, Does.Not.Contain(directions.Steps![0].Description));
    }

    [Test]
    public void GenerateHtml_includes_no_directions_if_not_available()
    {
        (var startingAddress, var destinationAddress, var directions, var weather, var imageCid) = CreateInput(withDirections: false);
        var html = new TemplateEngine().GenerateHtml(startingAddress, destinationAddress, directions, weather, imageCid);
        Assert.That(html, Does.Not.Contain("No Directions Available"));
        Assert.That(html, Does.Not.Contain("<li>"));
    }

    [Test]
    public void GenerateHtml_includes_weather_if_available()
    {
        (var startingAddress, var destinationAddress, var directions, var weather, var imageCid) = CreateInput();
        var html = new TemplateEngine().GenerateHtml(startingAddress, destinationAddress, directions, weather, imageCid);
        Assert.That(html, Does.Contain(weather.Items![0].ImageUrl));
        Assert.That(html, Does.Not.Contain("No Weather Available"));
    }

    [Test]
    public void GenerateHtml_includes_no_weather_if_not_successful()
    {
        (var startingAddress, var destinationAddress, var directions, var weather, var imageCid) = CreateInput(withWeatherSuccessful: false);
        var html = new TemplateEngine().GenerateHtml(startingAddress, destinationAddress, directions, weather, imageCid);
        Assert.That(html, Does.Contain("No Weather Available"));
        Assert.That(html, Does.Not.Contain(weather.Items![0].ImageUrl));
    }

    [Test]
    public void GenerateHtml_includes_no_weather_if_not_available()
    {
        (var startingAddress, var destinationAddress, var directions, var weather, var imageCid) = CreateInput(withWeather: false);
        var html = new TemplateEngine().GenerateHtml(startingAddress, destinationAddress, directions, weather, imageCid);
        Assert.That(html, Does.Not.Contain("No Weather Available"));
        Assert.That(html, Does.Not.Contain("class=\"MaxC\""));
    }

    [Test]
    public void GenerateHtml_includes_image_if_available()
    {
        (var startingAddress, var destinationAddress, var directions, var weather, var imageCid) = CreateInput();
        var html = new TemplateEngine().GenerateHtml(startingAddress, destinationAddress, directions, weather, imageCid);
        Assert.That(html, Does.Contain($"""<img src="cid:{imageCid}" />"""));
        Assert.That(html, Does.Not.Contain("No Image Available"));
    }

    [Test]
    public void GenerateHtml_includes_no_image_if_not_available()
    {
        (var startingAddress, var destinationAddress, var directions, var weather, var imageCid) = CreateInput(withImage: false);
        var html = new TemplateEngine().GenerateHtml(startingAddress, destinationAddress, directions, weather, imageCid);
        Assert.That(html, Does.Contain("No Image Available"));
        Assert.That(html, Does.Not.Contain($"""<img src="cid:{imageCid}" />"""));
    }

    private (string StartingAddress, string DestinationAddress, Directions Directions, WeatherForecast Weather, string? ImageCid) CreateInput(bool withDirectionsSuccessful = true, bool withDirections = true, bool withWeatherSuccessful = true, bool withWeather = true, bool withImage = true)
        => new
        (
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Build<Directions>().With(_ => _.IsSuccessful, withDirectionsSuccessful).With(_ => _.Steps, withDirections ? _fixture.CreateMany<DirectionsStep>().ToArray() : null).Create(),
            _fixture.Build<WeatherForecast>().With(_ => _.IsSuccessful, withWeatherSuccessful).With(_ => _.Items, withWeather ? _fixture.Create<WeatherForecastItem[]>() : null).Create(),
            withImage ? _fixture.Create<string>() : null
        );
}

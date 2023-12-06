using Email.Application.Templates;
using Microservices.Shared.Events;
using Scriban;
using System.Reflection;

namespace Email.Infrastructure.Templates;

/// <inheritdoc/>
public class TemplateEngine : ITemplateEngine
{
    /// <inheritdoc/>
    public string GenerateHtml(string startingAddress, string destinationAddress, Directions directions, WeatherForecast weather, string? imageCid)
    {
        var resourceName = Array.Find(Assembly.GetExecutingAssembly().GetManifestResourceNames(), _ => _.EndsWith("ProcessingComplete.html"));
        using var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName!);
        using var reader = new StreamReader(resource!);
        var source = reader.ReadToEnd();
        var template = Template.Parse(source);
        var model = new
        {
            start = startingAddress,
            destination = destinationAddress,
            hasdirections = directions.IsSuccessful,
            directions = directions.Steps,
            time = TimeSpan.FromSeconds(directions.TravelTimeSeconds.GetValueOrDefault(0)).ToString(),
            distance = directions.DistanceKm,
            hasweather = weather.IsSuccessful,
            forecast = weather.Items?.Select(_ => new { day = _.LocalTime.ToString("dddd"), date = _.LocalTime.ToString("M"), max = $"{_.MaximumTemperatureC:0.#}", min = $"{_.MinimumTemperatureC:0.#}", percentage = _.PrecipitationProbabilityPercentage, image = _.ImageUrl }),
            hasimage = !string.IsNullOrWhiteSpace(imageCid),
            image = imageCid
        };
        return template.Render(model);
    }
}

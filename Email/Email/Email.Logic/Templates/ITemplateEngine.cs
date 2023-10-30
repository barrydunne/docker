namespace Email.Logic.Templates
{
    /// <summary>
    /// Uses models and templates to render views.
    /// </summary>
    public interface ITemplateEngine
    {
        /// <summary>
        /// Generate the HTML body of the notification email to send.
        /// </summary>
        /// <param name="startingAddress">The starting location for the job.</param>
        /// <param name="destinationAddress">The destination location for the job.</param>
        /// <param name="directions">The directions between the locations.</param>
        /// <param name="weather">The weather forecast at the destination.</param>
        /// <param name="imageCid">The content id of the image, if available.</param>
        /// <returns>The rendered HTML.</returns>
        string GenerateHtml(string startingAddress, string destinationAddress, Microservices.Shared.Events.Directions directions, Microservices.Shared.Events.WeatherForecast weather, string? imageCid);
    }
}

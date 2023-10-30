namespace Microservices.Shared.Events
{
    /// <summary>
    /// The imaging result for the destination location.
    /// </summary>
    /// <param name="IsSuccessful">Whether the image was obtained successfully.</param>
    /// <param name="ImageUrl">The original URL of the image.</param>
    /// <param name="ImagePath">The path of the image in cloud file storage.</param>
    /// <param name="Error">The error if not successful.</param>
    public record ImagingResult(bool IsSuccessful, string? ImageUrl, string? ImagePath, string? Error);
}

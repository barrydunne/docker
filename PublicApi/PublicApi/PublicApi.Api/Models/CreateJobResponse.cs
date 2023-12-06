namespace PublicApi.Api.Models;

/// <summary>
/// The response from a new job request.
/// </summary>
public class CreateJobResponse
{
    /// <summary>
    /// Gets or sets the new job id.
    /// </summary>
    public Guid JobId { get; set; }
}

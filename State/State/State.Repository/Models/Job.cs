using Microservices.Shared.Events;

namespace State.Repository.Models
{
    /// <summary>
    /// Represents a processing job.
    /// </summary>
    public class Job
    {
        /// <summary>
        /// Gets or sets the correlation id for tracking this job.
        /// </summary>
        public Guid JobId { get; set; }

        /// <summary>
        /// Gets or sets the creation time of this job.
        /// </summary>
        public DateTime CreatedUtc { get; set; }

        /// <summary>
        /// Gets or sets the starting location for the job.
        /// </summary>
        public string StartingAddress { get; set; } = null!;

        /// <summary>
        /// Gets or sets the destination location for the job.
        /// </summary>
        public string DestinationAddress { get; set; } = null!;

        /// <summary>
        /// Gets or sets the notification email address for the job.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the success or failure outcome of geocoding.
        /// </summary>
        public bool? GeocodingSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the result of geocoding.
        /// </summary>
        public GeocodingResult? GeocodingResult { get; set; }

        /// <summary>
        /// Gets or sets the success or failure outcome of getting directions.
        /// </summary>
        public bool? DirectionsSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the result of getting directions.
        /// </summary>
        public Directions? Directions { get; set; }

        /// <summary>
        /// Gets or sets the success or failure outcome of getting the weather forecast.
        /// </summary>
        public bool? WeatherSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the result of getting the weather forecast.
        /// </summary>
        public WeatherForecast? WeatherForecast { get; set; }

        /// <summary>
        /// Gets or sets the success or failure outcome of imaging.
        /// </summary>
        public bool? ImagingSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the result of imaging.
        /// </summary>
        public ImagingResult? ImagingResult { get; set; }
    }
}

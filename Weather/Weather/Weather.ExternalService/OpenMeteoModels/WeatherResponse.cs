using System.Text.Json.Serialization;

namespace Weather.ExternalService.OpenMeteoModels
{
    // These classes were generated from the API response

    public class WeatherResponse
    {
        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }

        [JsonPropertyName("generationtime_ms")]
        public double? GenerationtimeMs { get; set; }

        [JsonPropertyName("utc_offset_seconds")]
        public int UtcOffsetSeconds { get; set; }

        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("timezone_abbreviation")]
        public string? TimezoneAbbreviation { get; set; }

        [JsonPropertyName("elevation")]
        public double? Elevation { get; set; }

        [JsonPropertyName("daily_units")]
        public WeatherResponseDailyUnits? DailyUnits { get; set; }

        [JsonPropertyName("daily")]
        public WeatherResponseDaily Daily { get; set; } = null!;

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
    }

    public class WeatherResponseDailyUnits
    {
        [JsonPropertyName("time")]
        public string? Time { get; set; }

        [JsonPropertyName("weathercode")]
        public string? WeatherCode { get; set; }

        [JsonPropertyName("temperature_2m_max")]
        public string? Temperature2mMax { get; set; }

        [JsonPropertyName("temperature_2m_min")]
        public string? Temperature2mMin { get; set; }

        [JsonPropertyName("precipitation_probability_max")]
        public string? PrecipitationProbabilityMax { get; set; }
    }

    public class WeatherResponseDaily
    {
        [JsonPropertyName("time")]
        public long[] Time { get; set; } = null!;

        [JsonPropertyName("weathercode")]
        public int[] WeatherCode { get; set; } = null!;

        [JsonPropertyName("temperature_2m_max")]
        public double[] Temperature2mMax { get; set; } = null!;

        [JsonPropertyName("temperature_2m_min")]
        public double[] Temperature2mMin { get; set; } = null!;

        [JsonPropertyName("precipitation_probability_max")]
        public int[] PrecipitationProbabilityMax { get; set; } = null!;
    }
}

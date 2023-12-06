using Email.Api.Models;
using Email.Application.Commands.SendEmail;
using Email.Application.Models;
using Email.Application.Queries.GetEmailsSentBetweenTimes;
using Email.Application.Queries.GetEmailsSentToRecipient;
using Mapster;
using Microservices.Shared.Events;

namespace Email.Api;

/// <summary>
/// Provides the auto-mapping configuration for type conversions.
/// </summary>
internal static class Mappings
{
    /// <summary>
    /// Configure the auto-mapping configuration for type conversions.
    /// </summary>
    internal static void Map()
    {
        TypeAdapterConfig<ProcessingCompleteEvent, SendEmailCommand>.NewConfig()
            .Map(dest => dest.JobId, src => src.JobId)
            .Map(dest => dest.Email, src => src.Email)
            .Map(dest => dest.StartingAddress, src => src.StartingAddress)
            .Map(dest => dest.DestinationAddress, src => src.DestinationAddress)
            .Map(dest => dest.Directions, src => src.Directions)
            .Map(dest => dest.Weather, src => src.Weather)
            .Map(dest => dest.Imaging, src => src.Imaging);

        TypeAdapterConfig<GetEmailsSentToRecipientRequest, GetEmailsSentToRecipientQuery>.NewConfig()
            .Map(dest => dest.Email, src => src.RecipientEmail)
            .Map(dest => dest.PageSize, src => src.PageSize)
            .Map(dest => dest.PageNumber, src => src.PageNumber);

        TypeAdapterConfig<GetEmailsSentBetweenTimesRequest, GetEmailsSentBetweenTimesQuery>.NewConfig()
            .Map(dest => dest.FromTime, src => DateTimeOffset.FromUnixTimeSeconds(src.FromUnixSeconds))
            .Map(dest => dest.ToTime, src => DateTimeOffset.FromUnixTimeSeconds(src.ToUnixSeconds))
            .Map(dest => dest.PageSize, src => src.PageSize)
            .Map(dest => dest.PageNumber, src => src.PageNumber);

        TypeAdapterConfig<SentEmail, EmailDetails>.NewConfig()
            .Map(dest => dest.JobId, src => src.JobId)
            .Map(dest => dest.RecipientEmail, src => src.RecipientEmail)
            .Map(dest => dest.SentTime, src => src.SentTime);

        TypeAdapterConfig<List<SentEmail>, SentEmailsResponse>.NewConfig()
            .Map(dest => dest.Emails, src => src.Select(_ => _.Adapt<EmailDetails>()).ToArray());

        // Fix for error: No default constructor for type 'WeatherForecastItem', please use 'ConstructUsing' or 'MapWith'
        TypeAdapterConfig<WeatherForecastItem, WeatherForecastItem>.NewConfig()
            .ConstructUsing(src => new WeatherForecastItem(src.ForecastTimeUnixSeconds, src.LocalTimeOffsetSeconds, src.WeatherCode, src.Description, src.ImageUrl, src.MinimumTemperatureC, src.MaximumTemperatureC, src.PrecipitationProbabilityPercentage));
    }
}

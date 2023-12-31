﻿using AspNet.KickStarter.CQRS.Abstractions.Queries;
using Microservices.Shared.Events;

namespace Weather.Application.Queries.GetWeather;

/// <summary>
/// Get the weather for a journey.
/// </summary>
/// <param name="JobId">The correlation id to include in logging when handling this query.</param>
/// <param name="Coordinates">The location for the job.</param>
public record GetWeatherQuery(Guid JobId, Coordinates Coordinates) : IQuery<WeatherForecast>;

using AspNet.KickStarter.CQRS.Abstractions.Commands;
using Microservices.Shared.Events;

namespace Directions.Application.Commands.GenerateDirections;

/// <summary>
/// Perform directions on two addresses.
/// </summary>
/// <param name="JobId">The correlation id to include in logging when handling this command.</param>
/// <param name="StartingCoordinates">The starting location for the job.</param>
/// <param name="DestinationCoordinates">The destination location for the job.</param>
public record GenerateDirectionsCommand(Guid JobId, Coordinates StartingCoordinates, Coordinates DestinationCoordinates) : ICommand;

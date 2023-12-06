using AspNet.KickStarter.CQRS.Abstractions.Commands;
using Microservices.Shared.Events;

namespace Imaging.Application.Commands.SaveImage;

/// <summary>
/// Obtain and save an image.
/// </summary>
/// <param name="JobId">The correlation id to include in logging when handling this command.</param>
/// <param name="Address">The target location address for the image.</param>
/// <param name="Coordinates">The target location coordinates for the image.</param>
public record SaveImageCommand(Guid JobId, string Address, Coordinates Coordinates) : ICommand;

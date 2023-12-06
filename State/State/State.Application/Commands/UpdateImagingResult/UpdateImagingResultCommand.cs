using AspNet.KickStarter.CQRS.Abstractions.Commands;
using Microservices.Shared.Events;

namespace State.Application.Commands.UpdateImagingResult;

/// <summary>
/// Record the imaging result and check if the job is complete.
/// </summary>
/// <param name="JobId">The correlation id for tracking this job.</param>
/// <param name="Imaging">The imaging result at the destination.</param>
public record UpdateImagingResultCommand(Guid JobId, ImagingResult Imaging) : ICommand;

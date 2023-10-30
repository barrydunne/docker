using AspNet.KickStarter.CQRS.Abstractions.Commands;

namespace Geocoding.Logic.Commands
{
    /// <summary>
    /// Perform geocoding on two addresses.
    /// </summary>
    /// <param name="JobId">The correlation id to include in logging when handling this command.</param>
    /// <param name="StartingAddress">The first address to geocode.</param>
    /// <param name="DestinationAddress">The second address to geocode.</param>
    public record GeocodeAddressesCommand(Guid JobId, string StartingAddress, string DestinationAddress) : ICommand;
}

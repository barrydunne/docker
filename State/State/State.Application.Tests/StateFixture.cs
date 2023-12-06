using Microservices.Shared.Mocks;

namespace State.Application.Tests;

internal class StateFixture : Fixture
{
    internal StateFixture() => Customizations.Add(new MicroserviceSpecimenBuilder());
}

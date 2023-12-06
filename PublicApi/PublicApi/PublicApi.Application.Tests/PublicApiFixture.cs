using Microservices.Shared.Mocks;

namespace PublicApi.Application.Tests;

internal class PublicApiFixture : Fixture
{
    internal PublicApiFixture() => Customizations.Add(new MicroserviceSpecimenBuilder());
}

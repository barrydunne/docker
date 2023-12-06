using Microservices.Shared.Mocks;

namespace Email.Application.Tests;

internal class EmailFixture : Fixture
{
    internal EmailFixture() => Customizations.Add(new MicroserviceSpecimenBuilder());
}

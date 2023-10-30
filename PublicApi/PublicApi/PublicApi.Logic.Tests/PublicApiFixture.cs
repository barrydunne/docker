using PublicApi.Logic.Commands;
using System.Net.Mail;

namespace PublicApi.Logic.Tests
{
    internal class PublicApiFixture : Fixture
    {
        internal PublicApiFixture() => Customize<CreateJobCommand>(_ => _.With(_ => _.Email, this.Create<MailAddress>().Address));
    }
}

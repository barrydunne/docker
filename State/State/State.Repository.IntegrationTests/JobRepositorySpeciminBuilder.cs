using AutoFixture.Kernel;
using Microservices.Shared.Mocks;
using State.Repository.Models;

namespace State.Repository.IntegrationTests
{
    internal class JobRepositorySpecimenBuilder : MicroserviceSpecimenBuilder
    {
        public override object Create(object request, ISpecimenContext context)
        {
            if ((request as Type) == typeof(Job))
            {
                var now = DateTime.UtcNow;
                var fixture = context.Resolve(typeof(IFixture)) as IFixture ?? throw new ApplicationException();
                return fixture.Build<Job>().With(_ => _.CreatedUtc, new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc)).Create();
            }

            return base.Create(request, context);
        }
    }
}

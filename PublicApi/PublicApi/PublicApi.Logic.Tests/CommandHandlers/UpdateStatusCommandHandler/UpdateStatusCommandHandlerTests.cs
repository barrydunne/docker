﻿using Microservices.Shared.Events;
using PublicApi.Logic.Commands;
using PublicApi.Logic.Tests.Mocks;
using PublicApi.Repository.Models;

namespace PublicApi.Logic.Tests.CommandHandlers.UpdateStatusCommandHandler
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    [Parallelizable(ParallelScope.Self)]
    [TestFixture(Category = "CommandHandlers")]
    internal class UpdateStatusCommandHandlerTests
    {
        private readonly PublicApiFixture _fixture = new();
        private readonly UpdateStatusCommandHandlerTestsContext _context = new();

        [Test]
        public async Task UpdateStatusCommandHandler_metrics_increments_count()
        {
            var command = _fixture.Create<UpdateStatusCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsCountIncremented();
        }

        [Test]
        public async Task UpdateStatusCommandHandler_metrics_records_guard_time()
        {
            var command = _fixture.Create<UpdateStatusCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsGuardTimeRecorded();
        }

        [Test]
        public async Task UpdateStatusCommandHandler_metrics_records_idempotency_time()
        {
            var command = _fixture.Create<UpdateStatusCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertMetricsUpdateTimeRecorded();
        }

        [Test]
        public async Task CreateJobCommandHandler_guards_fail_for_missing_job_id()
        {
            var command = _fixture.Build<UpdateStatusCommand>()
                                  .With(_ => _.JobId, Guid.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task CreateJobCommandHandler_guards_return_message_for_missing_job_id()
        {
            var command = _fixture.Build<UpdateStatusCommand>()
                                  .With(_ => _.JobId, Guid.Empty)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo("Required input JobId was empty. (Parameter 'JobId')"));
        }

        [Test]
        public async Task CreateJobCommandHandler_updates_status()
        {
            var job = _fixture.Create<Job>();

            var command = _fixture.Build<UpdateStatusCommand>()
                                  .With(_ => _.JobId, job.JobId)
                                  .Create();

            // Ensure updating to a different status
            while (command.Status == job.Status)
                job.Status = _fixture.Create<JobStatus>();

            _context.WithExistingJob(job);

            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertJobStatus(job.JobId, command.Status);
        }

        [Test]
        public async Task CreateJobCommandHandler_updates_additional_information()
        {
            var job = _fixture.Create<Job>();

            var command = _fixture.Build<UpdateStatusCommand>()
                                  .With(_ => _.JobId, job.JobId)
                                  .Create();

            // Ensure updating to a different status
            while (command.Status == job.Status)
                job.Status = _fixture.Create<JobStatus>();

            _context.WithExistingJob(job);

            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertJobAdditionalInformation(job.JobId, command.AdditionalInformation);
        }

        [Test]
        public async Task UpdateStatusCommandHandler_removes_job_from_cache()
        {
            var command = _fixture.Create<UpdateStatusCommand>();
            await _context.Sut.Handle(command, CancellationToken.None);
            _context.AssertJobRemovedFromCache(command.JobId);
        }

        [Test]
        public async Task UpdateStatusCommandHandler_returns_failure_on_exception()
        {
            var command = _fixture.Build<UpdateStatusCommand>()
                                  .With(_ => _.JobId, MockJobRepository.FailingJobId)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.IsFailure, Is.True);
        }

        [Test]
        public async Task UpdateStatusCommandHandler_returns_message_on_exception()
        {
            var command = _fixture.Build<UpdateStatusCommand>()
                                  .With(_ => _.JobId, MockJobRepository.FailingJobId)
                                  .Create();
            var result = await _context.Sut.Handle(command, CancellationToken.None);
            Assert.That(result.Error, Is.EqualTo(MockJobRepository.FailingJobIdError));
        }
    }
}

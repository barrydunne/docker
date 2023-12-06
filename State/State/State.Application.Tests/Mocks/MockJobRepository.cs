using Microservices.Shared.Events;
using Moq;
using State.Application.Models;
using State.Application.Repositories;

namespace State.Application.Tests.Mocks;

internal class MockJobRepository : Mock<IJobRepository>
{
    internal const string FailingStartingAddress = "FailingStartingAddress";
    internal static Guid FailingJobId => Guid.Parse("aaaaaBAD-DA7A-aaaa-aaaa-aaaaaaaaaaaa");
    internal const string FailingJobIdError = "FailingJobIdError";

    internal List<Job> Jobs { get; }

    internal MockJobRepository() : base(MockBehavior.Strict)
    {
        Jobs = new();

        Setup(_ => _.InsertAsync(It.IsAny<Job>(), It.IsAny<CancellationToken>()))
            .Callback((Job job, CancellationToken _) => AddJob(job))
            .Returns(Task.CompletedTask);

        Setup(_ => _.GetJobByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid jobId, CancellationToken _) => GetJob(jobId));

        Setup(_ => _.GetJobCompletionStatusAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid jobId, CancellationToken _) => GetJobCompletionStatus(jobId));

        Setup(_ => _.UpdateJobStatusAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<GeocodingResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid jobId, bool isSuccessful, GeocodingResult result, CancellationToken _) => UpdateJob(jobId, isSuccessful, result));

        Setup(_ => _.UpdateJobStatusAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<Directions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid jobId, bool isSuccessful, Directions result, CancellationToken _) => UpdateJob(jobId, isSuccessful, result));

        Setup(_ => _.UpdateJobStatusAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<WeatherForecast>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid jobId, bool isSuccessful, WeatherForecast result, CancellationToken _) => UpdateJob(jobId, isSuccessful, result));

        Setup(_ => _.UpdateJobStatusAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<ImagingResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid jobId, bool isSuccessful, ImagingResult result, CancellationToken _) => UpdateJob(jobId, isSuccessful, result));

        Setup(_ => _.DeleteJobByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid jobId, CancellationToken _) => DeleteJob(jobId));
    }

    internal void AddJob(Job job)
    {
        if (job.StartingAddress == FailingStartingAddress)
            throw new InvalidOperationException(FailingStartingAddress);
        Jobs.Add(job);
    }

    internal Job? GetJob(Guid jobId)
    {
        if (jobId == FailingJobId)
            throw new InvalidOperationException(FailingJobIdError);
        return Jobs.FirstOrDefault(_ => _.JobId == jobId);
    }

    internal bool? GetJobCompletionStatus(Guid jobId)
    {
        var job = GetJob(jobId);
        return (job?.DirectionsSuccessful is null) || (job.WeatherSuccessful is null) || (job.ImagingSuccessful is null)
            ? null
            : (job.DirectionsSuccessful == true) && (job.WeatherSuccessful == true) && (job.ImagingSuccessful == true);
    }

    internal long UpdateJob(Guid jobId, bool isSuccessful, GeocodingResult result)
    {
        var job = GetJob(jobId);
        if (job is null)
            return 0;
        job.GeocodingSuccessful = isSuccessful;
        job.GeocodingResult = result;
        return 1;
    }

    internal long UpdateJob(Guid jobId, bool isSuccessful, Directions result)
    {
        var job = GetJob(jobId);
        if (job is null)
            return 0;
        job.DirectionsSuccessful = isSuccessful;
        job.Directions = result;
        return 1;
    }

    internal long UpdateJob(Guid jobId, bool isSuccessful, WeatherForecast result)
    {
        var job = GetJob(jobId);
        if (job is null)
            return 0;
        job.WeatherSuccessful = isSuccessful;
        job.WeatherForecast = result;
        return 1;
    }

    internal long UpdateJob(Guid jobId, bool isSuccessful, ImagingResult result)
    {
        var job = GetJob(jobId);
        if (job is null)
            return 0;
        job.ImagingSuccessful = isSuccessful;
        job.ImagingResult = result;
        return 1;
    }

    internal long DeleteJob(Guid jobId)
    {
        var job = GetJob(jobId);
        if (job is null)
            return 0;
        Jobs.Remove(job);
        return 1;
    }
}

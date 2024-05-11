using Microservices.Shared.Events;
using State.Application.Models;
using State.Application.Repositories;

namespace State.Application.Tests.Mocks;

internal class MockJobRepository : IJobRepository
{
    internal const string FailingStartingAddress = "FailingStartingAddress";
    internal static Guid FailingJobId => Guid.Parse("aaaaaBAD-DA7A-aaaa-aaaa-aaaaaaaaaaaa");
    internal const string FailingJobIdError = "FailingJobIdError";
    private Exception? _readException;
    private Exception? _writeException;

    internal List<Job> Jobs { get; }

    internal MockJobRepository() => Jobs = new();

    internal void WithReadException(Exception? exception = null) => _readException = exception ?? new InvalidOperationException();

    internal void WithWriteException(Exception? exception = null) => _writeException = exception ?? new InvalidOperationException();

    public Task<long> DeleteJobByIdAsync(Guid jobId, CancellationToken cancellationToken = default)
        => Task.FromResult(DeleteJob(jobId));

    public Task<Job?> GetJobByIdAsync(Guid jobId, CancellationToken cancellationToken = default)
        => Task.FromResult(GetJob(jobId));

    public Task<bool?> GetJobCompletionStatusAsync(Guid jobId, CancellationToken cancellationToken = default)
        => Task.FromResult(GetJobCompletionStatus(jobId));

    public Task InsertAsync(Job job, CancellationToken cancellationToken = default)
    {
        AddJob(job);
        return Task.CompletedTask;
    }

    public Task<long> UpdateJobStatusAsync(Guid jobId, bool isSuccessful, GeocodingResult result, CancellationToken cancellationToken = default)
         => Task.FromResult(UpdateJob(jobId, isSuccessful, result));

    public Task<long> UpdateJobStatusAsync(Guid jobId, bool isSuccessful, Directions result, CancellationToken cancellationToken = default)
        => Task.FromResult(UpdateJob(jobId, isSuccessful, result));

    public Task<long> UpdateJobStatusAsync(Guid jobId, bool isSuccessful, WeatherForecast result, CancellationToken cancellationToken = default)
        => Task.FromResult(UpdateJob(jobId, isSuccessful, result));
    public Task<long> UpdateJobStatusAsync(Guid jobId, bool isSuccessful, ImagingResult result, CancellationToken cancellationToken = default)
        => Task.FromResult(UpdateJob(jobId, isSuccessful, result));

    internal void AddJob(Job job)
    {
        if (_writeException is not null)
            throw _writeException;
        if (job.StartingAddress == FailingStartingAddress)
            throw new InvalidOperationException(FailingStartingAddress);
        Jobs.Add(job);
    }

    internal Job? GetJob(Guid jobId)
    {
        if (_readException is not null)
            throw _readException;
        if (jobId == FailingJobId)
            throw new InvalidOperationException(FailingJobIdError);
        return Jobs.FirstOrDefault(_ => _.JobId == jobId);
    }

    internal bool? GetJobCompletionStatus(Guid jobId)
    {
        if (_readException is not null)
            throw _readException;
        var job = GetJob(jobId);
        return (job?.DirectionsSuccessful is null) || (job.WeatherSuccessful is null) || (job.ImagingSuccessful is null)
            ? null
            : (job.DirectionsSuccessful == true) && (job.WeatherSuccessful == true) && (job.ImagingSuccessful == true);
    }

    internal long UpdateJob(Guid jobId, bool isSuccessful, GeocodingResult result)
    {
        if (_writeException is not null)
            throw _writeException;
        var job = GetJob(jobId);
        if (job is null)
            return 0;
        job.GeocodingSuccessful = isSuccessful;
        job.GeocodingResult = result;
        return 1;
    }

    internal long UpdateJob(Guid jobId, bool isSuccessful, Directions result)
    {
        if (_writeException is not null)
            throw _writeException;
        var job = GetJob(jobId);
        if (job is null)
            return 0;
        job.DirectionsSuccessful = isSuccessful;
        job.Directions = result;
        return 1;
    }

    internal long UpdateJob(Guid jobId, bool isSuccessful, WeatherForecast result)
    {
        if (_writeException is not null)
            throw _writeException;
        var job = GetJob(jobId);
        if (job is null)
            return 0;
        job.WeatherSuccessful = isSuccessful;
        job.WeatherForecast = result;
        return 1;
    }

    internal long UpdateJob(Guid jobId, bool isSuccessful, ImagingResult result)
    {
        if (_writeException is not null)
            throw _writeException;
        var job = GetJob(jobId);
        if (job is null)
            return 0;
        job.ImagingSuccessful = isSuccessful;
        job.ImagingResult = result;
        return 1;
    }

    internal long DeleteJob(Guid jobId)
    {
        if (_writeException is not null)
            throw _writeException;
        var job = GetJob(jobId);
        if (job is null)
            return 0;
        Jobs.Remove(job);
        return 1;
    }
}

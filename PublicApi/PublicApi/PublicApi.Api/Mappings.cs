using Mapster;
using Microservices.Shared.Events;
using PublicApi.Api.Models;
using PublicApi.Application.Commands.CreateJob;
using PublicApi.Application.Commands.UpdateStatus;
using PublicApi.Application.Models;
using PublicApi.Application.Queries.GetJobStatus;

namespace PublicApi.Api;

/// <summary>
/// Provides the auto-mapping configuration for type conversions.
/// </summary>
internal static class Mappings
{
    /// <summary>
    /// Configure the auto-mapping configuration for type conversions.
    /// </summary>
    internal static void Map()
    {
        TypeAdapterConfig<JobStatusUpdateEvent, UpdateStatusCommand>.NewConfig()
            .Map(dest => dest.JobId, src => src.JobId)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.AdditionalInformation, src => src.Details);

        TypeAdapterConfig<(string IdempotencyKey, CreateJobRequest Request), CreateJobCommand>.NewConfig()
            .Map(dest => dest.IdempotencyKey, src => src.IdempotencyKey)
            .Map(dest => dest.StartingAddress, src => src.Request.StartingAddress)
            .Map(dest => dest.DestinationAddress, src => src.Request.DestinationAddress)
            .Map(dest => dest.Email, src => src.Request.Email);

        TypeAdapterConfig<Guid, CreateJobResponse>.NewConfig()
            .Map(dest => dest.JobId, src => src);

        TypeAdapterConfig<Guid, GetJobStatusQuery>.NewConfig()
            .Map(dest => dest.JobId, src => src);

        TypeAdapterConfig<Job, GetJobStatusResponse>.NewConfig()
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.AdditionalInformation, src => src.AdditionalInformation);
    }
}

using Polly;
using RestSharp;

namespace Microservices.Shared.RestSharpFactory;

/// <summary>
/// Provides a resilience pipeline for RestSharp responses.
/// </summary>
public interface IRestSharpResiliencePipeline
{
    /// <summary>
    /// Get the resilient pipeline to use to execute requests.
    /// </summary>
    /// <returns>The resilient pipeline to use to execute requests.</returns>
    ResiliencePipeline<RestResponse> GetPipeline();
}

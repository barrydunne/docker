using Microservices.Shared.RestSharpFactory;
using Polly;
using RestSharp;

namespace Microservices.Shared.Mocks;

public class MockRestSharpResiliencePipeline : IRestSharpResiliencePipeline
{
    private readonly ResiliencePipeline<RestResponse> _pipeline;

    public MockRestSharpResiliencePipeline() => _pipeline = new ResiliencePipelineBuilder<RestResponse>().Build();

    public ResiliencePipeline<RestResponse> GetPipeline() => _pipeline;
}

using Imaging.Application.Commands.SaveImage;
using Microsoft.Extensions.Logging;
using Moq;

namespace Imaging.Application.Tests.Commands.SaveImage;

internal class SaveImageCommandValidatorTestsContext
{
    private readonly Mock<ISaveImageCommandHandlerMetrics> _mockMetrics;

    internal SaveImageCommandValidator Sut { get; }

    public SaveImageCommandValidatorTestsContext()
    {
        _mockMetrics = new();
        Sut = new(_mockMetrics.Object, new Mock<ILogger<SaveImageCommandValidator>>().Object);
    }

    internal SaveImageCommandValidatorTestsContext AssertMetricsGuardTimeRecorded()
    {
        _mockMetrics.Verify(_ => _.RecordGuardTime(It.IsAny<double>()), Times.Once);
        return this;
    }
}

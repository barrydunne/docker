﻿using Email.Application.Commands.SendEmail;
using Email.Application.Templates;
using Email.Application.Tests.Mocks;
using Microservices.Shared.Events;
using Microservices.Shared.Mocks;

namespace Email.Application.Tests.Commands.SendEmail;

internal class SendEmailCommandHandlerTestsContext
{
    private readonly Fixture _fixture;
    private readonly MockCloudEmail _mockCloudEmail;
    private readonly MockCloudFiles _mockCloudFiles;
    private readonly MockEmailRepository _mockEmailRepository;
    private readonly ITemplateEngine _mockTemplateEngine;
    private readonly ISendEmailCommandHandlerMetrics _mockMetrics;
    private readonly MockLogger<SendEmailCommandHandler> _mockLogger;

    internal SendEmailCommandHandler Sut { get; }

    public SendEmailCommandHandlerTestsContext()
    {
        _fixture = new();
        _mockCloudEmail = new();
        _mockCloudFiles = new();
        _mockEmailRepository = new();
        _mockTemplateEngine = Substitute.For<ITemplateEngine>();
        _mockMetrics = Substitute.For<ISendEmailCommandHandlerMetrics>();
        _mockLogger = new();

        Sut = new(_mockCloudEmail, _mockCloudFiles, _mockEmailRepository, _mockTemplateEngine, _mockMetrics, _mockLogger);
    }

    internal SendEmailCommandHandlerTestsContext WithImage(string path)
    {
        var image = Convert.FromBase64String("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");
        using var stream = new MemoryStream(image);
        _mockCloudFiles.AddFile("Imaging", path, stream);
        return this;
    }

    internal SendEmailCommandHandlerTestsContext WithSendFailure()
    {
        _mockCloudEmail.WithSendFailure();
        return this;
    }

    internal SendEmailCommandHandlerTestsContext WithSendException()
    {
        _mockCloudEmail.WithSendException();
        return this;
    }

    internal SendEmailCommandHandlerTestsContext AssertMetricsCountIncremented()
    {
        _mockMetrics.Received(1).IncrementCount();
        return this;
    }

    internal SendEmailCommandHandlerTestsContext AssertMetricsImageTimeRecorded()
    {
        _mockMetrics.Received(1).RecordImageTime(Arg.Any<double>());
        return this;
    }

    internal SendEmailCommandHandlerTestsContext AssertMetricsGenerateTimeRecorded()
    {
        _mockMetrics.Received(1).RecordGenerateTime(Arg.Any<double>());
        return this;
    }

    internal SendEmailCommandHandlerTestsContext AssertMetricsEmailTimeRecorded()
    {
        _mockMetrics.Received(1).RecordEmailTime(Arg.Any<double>());
        return this;
    }

    internal SendEmailCommandHandlerTestsContext AssertHtmlGeneratedWithoutImage()
    {
        _mockTemplateEngine.Received(1).GenerateHtml(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Directions>(), Arg.Any<WeatherForecast>(), Arg.Is((string?)null));
        return this;
    }

    internal SendEmailCommandHandlerTestsContext AssertHtmlGeneratedWithImage()
    {
        _mockTemplateEngine.Received(0).GenerateHtml(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Directions>(), Arg.Any<WeatherForecast>(), Arg.Is((string?)null));
        _mockTemplateEngine.Received(1).GenerateHtml(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Directions>(), Arg.Any<WeatherForecast>(), Arg.Any<string>());
        return this;
    }

    internal SendEmailCommandHandlerTestsContext AssertEmailSentWithoutImage()
    {
        _mockCloudEmail.Emails.Count.ShouldBe(1, "No email sent");
        var (_, _, _, _, _, _, images) = _mockCloudEmail.Emails.First();
        images.ShouldBeEmpty("Image sent");
        return this;
    }

    internal SendEmailCommandHandlerTestsContext AssertEmailSentWithImage()
    {
        _mockCloudEmail.Emails.Count.ShouldBe(1, "No email sent");
        var (_, _, _, _, _, _, images) = _mockCloudEmail.Emails.First();
        images.Count().ShouldBe(1, "Image not sent");
        return this;
    }
}

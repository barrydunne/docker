using FluentFTP;
using Microsoft.Extensions.Logging;

namespace Microservices.Shared.CloudFiles.Ftp
{
    /// <summary>
    /// Logs file transfer progress.
    /// </summary>
    internal class ProgressReporter : IProgress<FtpProgress>
    {
        private readonly ILogger _logger;

        private string? _lastReported = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressReporter"/> class.
        /// </summary>
        /// <param name="logger">The logger to write to.</param>
        public ProgressReporter(ILogger logger) => _logger = logger;

        /// <inheritdoc/>
        public void Report(FtpProgress value)
        {
            var progress = $"Transfer: {value.Progress:N0}%";
            if (_lastReported != progress)
            {
                _logger.LogDebug(progress);
                _lastReported = progress;
            }
        }
    }
}

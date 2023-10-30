using System.Diagnostics.CodeAnalysis;

namespace Microservices.Shared.CloudFiles.Ftp
{
    /// <summary>
    /// The connection options for FTP.
    /// Currently only anonymous connections are supported.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FtpFilesOptions
    {
        /// <summary>
        /// Gets or sets the hostname for the FTP connection.
        /// </summary>
        public string Host { get; set; } = "localhost";

        /// <summary>
        /// Gets or sets the port number for the FTP connection.
        /// </summary>
        public int Port { get; set; } = 21;

        /// <summary>
        /// Gets or sets the remote directory containing all files.
        /// </summary>
        public string BaseDir { get; set; } = "/files";
    }
}

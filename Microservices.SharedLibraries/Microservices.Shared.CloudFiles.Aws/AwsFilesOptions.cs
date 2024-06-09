using System.Diagnostics.CodeAnalysis;

namespace Microservices.Shared.CloudFiles.Aws;

/// <summary>
/// The configuration options for S3.
/// </summary>
[ExcludeFromCodeCoverage]
public class AwsFilesOptions
{
    /// <summary>
    /// Gets or sets the S3 region.
    /// </summary>
    public string Region { get; set; } = "eu-west-1";
}

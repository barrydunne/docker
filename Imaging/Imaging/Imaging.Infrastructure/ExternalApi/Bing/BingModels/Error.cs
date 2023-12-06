using System.Text.Json.Serialization;

namespace Imaging.Infrastructure.ExternalApi.Bing.BingModels;

/// <summary>
/// Defines the error that occurred.
/// </summary>
public class Error
{
    /// <summary>
    /// Gets or sets the error code that identifies the category of error. Possible values include:
    /// 'None', 'ServerError', 'InvalidRequest', 'RateLimitExceeded', 'InvalidAuthorization', 'InsufficientAuthorization'.
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets the error code that further helps to identify the error. Possible values include:
    /// 'UnexpectedError', 'ResourceError', 'NotImplemented', 'ParameterMissing', 'ParameterInvalidValue', 'HttpNotAllowed', 'Blocked',
    /// 'AuthorizationMissing', 'AuthorizationRedundancy', 'AuthorizationDisabled', 'AuthorizationExpired'.
    /// </summary>
    [JsonPropertyName("subCode")]
    public string? SubCode { get; set; }

    /// <summary>
    /// Gets or sets a description of the error.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets a description that provides additional information about the error.
    /// </summary>
    [JsonPropertyName("moreDetails")]
    public string? MoreDetails { get; set; }

    /// <summary>
    /// Gets or sets the parameter in the request that caused the error.
    /// </summary>
    [JsonPropertyName("parameter")]
    public string? Parameter { get; set; }

    /// <summary>
    /// Gets or sets the parameter's value in the request that was not valid.
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

using System.Diagnostics.CodeAnalysis;

namespace Directions.Application;

/// <summary>
/// A problem has occurred that prevented directions from completing successfully.
/// </summary>
[Serializable]
[ExcludeFromCodeCoverage]
[SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "Exception(SerializationInfo info, StreamingContext context) is Obsolete")]
public class DirectionsException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DirectionsException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public DirectionsException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
}

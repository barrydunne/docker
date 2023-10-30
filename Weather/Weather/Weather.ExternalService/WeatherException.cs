using System.Diagnostics.CodeAnalysis;

namespace Weather.ExternalService
{
    /// <summary>
    /// A problem has occurred that prevented weather from completing successfully.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "Exception(SerializationInfo info, StreamingContext context) is Obsolete")]
    public class WeatherException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public WeatherException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
    }
}

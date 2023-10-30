using System.Diagnostics.CodeAnalysis;

namespace Imaging.ExternalService
{
    /// <summary>
    /// A problem has occurred that prevented imaging from completing successfully.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "Exception(SerializationInfo info, StreamingContext context) is Obsolete")]
    public class ImagingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImagingException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ImagingException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
    }
}

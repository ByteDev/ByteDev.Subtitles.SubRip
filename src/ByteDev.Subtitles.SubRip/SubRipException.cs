using System;
using System.Runtime.Serialization;

namespace ByteDev.Subtitles.SubRip
{
    /// <summary>
    /// Represents an error when handling SubRip file.
    /// </summary>
    [Serializable]
    public class SubRipException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Subtitles.SubRip.SubRipException" /> class.
        /// </summary>
        public SubRipException() : base("Error occured handling SubRip file.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Subtitles.SubRip.SubRipException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SubRipException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Subtitles.SubRip.SubRipException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>       
        public SubRipException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ByteDev.Subtitles.SubRip.SubRipException" /> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected SubRipException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
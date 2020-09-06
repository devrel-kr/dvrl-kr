using System;
using System.Runtime.Serialization;

namespace DevRelKr.UrlShortener.Domains.Exceptions
{
    /// <summary>
    /// This represents the exception entity that the given short URL already exists in the data store.
    /// </summary>
    public class UrlExistsException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UrlExistsException"/> class.
        /// </summary>
        public UrlExistsException()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlExistsException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public UrlExistsException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlExistsException"/> class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner <see cref="Exception"/> instance.</param>
        public UrlExistsException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlExistsException"/> class.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> instance.</param>
        /// <param name="context"><see cref="StreamingContext"/> instance.</param>
        protected UrlExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        /// <summary>
        /// Gets or sets the short URL that threw the exception.
        /// </summary>
        public virtual string ShortUrl { get; set; }
    }
}

using Newtonsoft.Json;

namespace DevRelKr.UrlShortener.Models.Responses
{
    /// <summary>
    /// This represents the response entity for exceptions.
    /// /// </summary>
    public class ExceptionResponse
    {
        /// <summary>
        /// Gets or sets the exception message.
        /// </summary>
        [JsonProperty("message")]
        public virtual string Message { get; set; }

        /// <summary>
        /// Gets or sets the exception stack trace.
        /// </summary>
        [JsonProperty("stackTrace")]
        public virtual string StackTrace { get; set; }
    }
}

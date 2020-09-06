using Newtonsoft.Json;

namespace DevRelKr.UrlShortener.Models.Requests
{
    /// <summary>
    /// This represents the request entity for URL expander.
    /// </summary>
    public class ExpanderRequest
    {
        /// <summary>
        /// Gets or sets the short URL.
        /// </summary>
        [JsonRequired]
        public virtual string ShortUrl { get; set; }
    }
}

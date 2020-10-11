using System.Collections.Generic;

using DevRelKr.UrlShortener.Models.DataStores;

using Newtonsoft.Json;

namespace DevRelKr.UrlShortener.Models.Responses
{
    /// <summary>
    /// This represents the response entity for URL expander result.
    /// </summary>
    public class ExpanderResponse : UrlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpanderResponse"/> class.
        /// </summary>
        public ExpanderResponse()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpanderResponse"/> class.
        /// </summary>
        /// <param name="item"><see cref="UrlItemEntity"/> object.</param>
        public ExpanderResponse(UrlItemEntity item)
            : base()
        {
            this.EntityId = item.EntityId;
            this.Original = item.OriginalUrl;
            this.ShortUrl = item.ShortUrl;
            this.Title = item.Title;
            this.Description = item.Description;
            this.Owner = item.Owner;
            this.CoOwners = item.CoOwners;
            this.DateGenerated = item.DateGenerated;
            this.DateUpdated = item.DateUpdated;
            this.HitCount = item.HitCount;
        }

        /// <summary>
        /// Gets or sets the request header values from the expander requests.
        /// </summary>
        [JsonIgnore]
        public virtual Dictionary<string, object> RequestHeaders { get; set; } = new Dictionary<string, object>();
    }
}

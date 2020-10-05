using System.Collections.Generic;

using DevRelKr.UrlShortener.Models.Responses;

using Newtonsoft.Json;

namespace DevRelKr.UrlShortener.Models.DataStores
{
    /// <summary>
    /// This represents the item entity for Visit.
    /// </summary>
    public class VisitItemEntity : ItemEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisitItemEntity"/> class.
        /// </summary>
        public VisitItemEntity()
            : base()
        {
            this.Collection = PartitionType.Visit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisitItemEntity"/> class.
        /// </summary>
        /// <param name="payload"><see cref="UrlResponse"/> object.</param>
        public VisitItemEntity(UrlResponse payload)
            : this()
        {
            this.EntityId = payload.EntityId;
            this.ShortUrl = payload.ShortUrl;
            this.DateGenerated = payload.DateGenerated;

            this.RequestHeaders = (payload as ExpanderResponse).RequestHeaders;
        }

        /// <summary>
        /// Gets or sets the short URL.
        /// </summary>
        [JsonRequired]
        [JsonProperty("shortUrl")]
        public virtual string ShortUrl { get; set; }

        /// <summary>
        /// Gets or sets the request headers.
        /// </summary>
        [JsonRequired]
        [JsonProperty("requestHeaders")]
        public virtual Dictionary<string, object> RequestHeaders { get; set; } = new Dictionary<string, object>();
    }
}

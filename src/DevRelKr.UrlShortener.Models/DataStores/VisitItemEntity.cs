using System;
using System.Collections.Generic;

using DevRelKr.UrlShortener.Models.Responses;

using Microsoft.Extensions.Primitives;

using Newtonsoft.Json;

namespace DevRelKr.UrlShortener.Models.DataStores
{
    /// <summary>
    /// This represents the item entity for Visit.
    /// </summary>
    public class VisitItemEntity : ItemEntity
    {
        private Guid _urlId;

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

            this.UrlId = (payload as ExpanderResponse).UrlId;
            this.RequestHeaders = (payload as ExpanderResponse).RequestHeaders;
            this.RequestQueries = (payload as ExpanderResponse).RequestQueries;
        }

        /// <summary>
        /// Gets or sets the EntityId of the <see cref="UrlItemEntity"/> object.
        /// </summary>
        [JsonRequired]
        [JsonProperty("urlId")]
        public virtual Guid UrlId
        {
            get { return this._urlId; }
            set
            {
                if (value == Guid.Empty)
                {
                    throw new InvalidOperationException("UrlId: Value not allowed");
                }

                this._urlId = value;
            }
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

        /// <summary>
        /// Gets or sets the request querystring.
        /// </summary>
        [JsonRequired]
        [JsonProperty("requestQueries")]
        public virtual Dictionary<string, StringValues> RequestQueries { get; set; } = new Dictionary<string, StringValues>();
    }
}

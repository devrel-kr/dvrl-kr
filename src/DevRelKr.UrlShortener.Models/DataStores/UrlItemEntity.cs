using System;
using System.Collections.Generic;

using DevRelKr.UrlShortener.Models.Responses;

using Newtonsoft.Json;

namespace DevRelKr.UrlShortener.Models.DataStores
{
    /// <summary>
    /// This represents the item entity for URL.
    /// </summary>
    public class UrlItemEntity : ItemEntity
    {
        private DateTimeOffset _dateUpdated;

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlItemEntity"/> class.
        /// </summary>
        public UrlItemEntity()
            : base()
        {
            this.Collection = PartitionType.Url;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UrlItemEntity"/> class.
        /// </summary>
        /// <param name="payload"><see cref="UrlResponse"/> object.</param>
        public UrlItemEntity(UrlResponse payload)
            : this()
        {
            this.EntityId = payload.EntityId;
            this.ShortUrl = payload.ShortUrl;
            this.OriginalUrl = payload.Original;
            this.Title = payload.Title;
            this.Description = payload.Description;
            this.Owner = payload.Owner;
            this.CoOwners = payload.CoOwners;
            this.DateGenerated = payload.DateGenerated;
            this.DateUpdated = payload.DateUpdated;
            this.HitCount = payload.HitCount;
        }

        /// <summary>
        /// Gets or sets the short URL.
        /// </summary>
        [JsonRequired]
        [JsonProperty("shortUrl")]
        public virtual string ShortUrl { get; set; }

        /// <summary>
        /// Gets or sets the original URL.
        /// </summary>
        [JsonRequired]
        [JsonProperty("originalUrl")]
        public virtual Uri OriginalUrl { get; set; }

        /// <summary>
        /// Gets or sets the title of the shortened URL.
        /// </summary>
        [JsonProperty("title")]
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the description of the shortened URL.
        /// </summary>
        [JsonProperty("description")]
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the owner of the short URL.
        /// </summary>
        [JsonRequired]
        [JsonProperty("owner")]
        public virtual string Owner { get; set; }

        /// <summary>
        /// Gets or sets the list of co-owners of the short URL.
        /// </summary>
        [JsonProperty("coOwners")]
        public virtual List<string> CoOwners { get;  set; } = new List<string>();

        /// <summary>
        /// Gets or sets the date/time when the shortened URL was updated.
        /// </summary>
        [JsonRequired]
        [JsonProperty("dateUpdated")]
        public virtual DateTimeOffset DateUpdated
        {
            get { return this._dateUpdated; }
            set
            {
                if (value == DateTimeOffset.MinValue)
                {
                    throw new InvalidOperationException("DateUpdated: Value not allowed");
                }

                this._dateUpdated = value;
            }
        }

        /// <summary>
        /// Gets or sets the hit count of the URL.
        /// </summary>
        [JsonProperty("hitCount")]
        public virtual int HitCount { get; set; }
    }
}

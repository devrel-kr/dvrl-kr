using System;
using System.Collections.Generic;

namespace DevRelKr.UrlShortener.Models.Responses
{
    /// <summary>
    /// This represents the response entity for URL shortening/expanding result.
    /// </summary>
    public abstract class UrlResponse
    {
        /// <summary>
        /// Gets or sets the entity ID.
        /// </summary>
        public virtual Guid EntityId { get; set; }

        /// <summary>
        /// Gets or sets the original <see cref="Uri"/>.
        /// </summary>
        public virtual Uri Original { get; set; }

        /// <summary>
        /// Gets or sets the shortened <see cref="Uri"/>.
        /// </summary>
        public virtual Uri Shortened { get; set; }

        /// <summary>
        /// Gets or sets the short URL.
        /// </summary>
        public virtual string ShortUrl { get; set; }

        /// <summary>
        /// Gets or sets the title of the shortened URL.
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the description of the shortened URL.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the owner of the shortened URL.
        /// </summary>
        public virtual string Owner { get; set; }

        /// <summary>
        /// Gets or sets the list of co-owners of the shortened URL.
        /// </summary>
        public virtual List<string> CoOwners { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the date/time when the shortened URL was generated.
        /// </summary>
        public virtual DateTimeOffset DateGenerated { get; set; }

        /// <summary>
        /// Gets or sets the date/time when the shortened URL was updated.
        /// </summary>
        public virtual DateTimeOffset DateUpdated { get; set; }

        /// <summary>
        /// Gets or sets the hit count of the URL.
        /// </summary>
        public virtual int HitCount { get; set; }
    }
}

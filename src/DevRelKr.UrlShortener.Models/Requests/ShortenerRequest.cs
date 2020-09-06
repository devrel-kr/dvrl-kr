using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace DevRelKr.UrlShortener.Models.Requests
{
    /// <summary>
    /// This represents the request entity for URL shortener.
    /// </summary>
    public class ShortenerRequest
    {
        /// <summary>
        /// Gets or sets the original <see cref="Uri"/>.
        /// </summary>
        [JsonRequired]
        public virtual Uri Original { get; set; }

        private string _friendly;

        /// <summary>
        /// Gets or sets the friendly URL.
        /// </summary>
        public virtual string Friendly
        {
            get
            {
                return this._friendly;
            }
            set
            {
                this._friendly = string.IsNullOrWhiteSpace(value) ? value : value.TrimEnd('/');
            }
        }

        /// <summary>
        /// Gets or sets the title of the shortened URL.
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the description of the shortened URL.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the owner of the short URL.
        /// </summary>
        [JsonRequired]
        public virtual string Owner { get; set; }

        /// <summary>
        /// Gets or sets the list of co-owners of the short URL.
        /// </summary>
        public virtual List<string> CoOwners { get;  set; } = new List<string>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Domains.Exceptions;
using DevRelKr.UrlShortener.Domains.Extensions;
using DevRelKr.UrlShortener.Models.DataStores;
using DevRelKr.UrlShortener.Models.Requests;
using DevRelKr.UrlShortener.Models.Responses;
using DevRelKr.UrlShortener.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace DevRelKr.UrlShortener.Domains
{
    /// <summary>
    /// This represents the domain entity for URL.
    /// </summary>
    public class Url : IUrl
    {
        private readonly IShortenerService _shortener;
        private readonly IExpanderService _expander;

        /// <summary>
        /// Initializes a new instance of the <see cref="Url"/> class.
        /// </summary>
        /// <param name="shortener"><see cref="IShortenerService"/> instance.</param>
        /// <param name="expander"><see cref="IExpanderService"/> instance.</param>
        public Url(IShortenerService shortener, IExpanderService expander)
        {
            this._shortener = shortener ?? throw new ArgumentNullException(nameof(shortener));
            this._expander = expander ?? throw new ArgumentNullException(nameof(expander));
        }

        /// <inheritdoc/>
        public Guid EntityId { get; private set; }

        /// <inheritdoc/>
        public Guid UrlId { get; private set; }

        /// <inheritdoc/>
        public Uri Original { get; private set; }

        /// <inheritdoc/>
        public Uri Shortened { get; private set; }

        /// <inheritdoc/>
        public string ShortUrl { get; private set; }

        /// <inheritdoc/>
        public bool IsFriendlyUrlValidated { get; private set; }

        /// <inheritdoc/>
        public bool IsRandomUrlValidated { get; private set; }

        /// <inheritdoc/>
        public string Title { get; private set; }

        /// <inheritdoc/>
        public string Description { get; private set; }

        /// <inheritdoc/>
        public string Owner { get; private set; }

        /// <inheritdoc/>
        public List<string> CoOwners { get; private set; } = new List<string>();

        /// <inheritdoc/>
        public DateTimeOffset DateGenerated { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset DateUpdated { get; private set; }

        /// <inheritdoc/>
        public int HitCount { get; private set; }

        /// <inheritdoc/>
        public ShortenerRequest ShortenerRequest { get; private set; }

        /// <inheritdoc/>
        public ShortenerResponse ShortenerResponse { get; private set; }

        /// <inheritdoc/>
        public ExpanderRequest ExpanderRequest { get; private set; }

        /// <inheritdoc/>
        public Dictionary<string, object> ExpanderRequestHeaders { get; private set; } = new Dictionary<string, object>();

        /// <inheritdoc/>
        public Dictionary<string, StringValues> ExpanderRequestQueries { get; private set; } = new Dictionary<string, StringValues>();

        /// <inheritdoc/>
        public ExpanderResponse ExpanderResponse { get; private set; }

        /// <inheritdoc/>
        public async Task<IUrl> GetRequestAsync(HttpRequest req)
        {
            if (req == null)
            {
                throw new ArgumentNullException(nameof(req));
            }

            var request = await req.GetShortenerRequestAsync().ConfigureAwait(false);

            this.ShortenerRequest = request;

            return this;
        }

        /// <inheritdoc/>
        public async Task<IUrl> GetRequestAsync(HttpRequest req, string shortUrl)
        {
            if (req == null)
            {
                throw new ArgumentNullException(nameof(req));
            }

            if (string.IsNullOrWhiteSpace(shortUrl))
            {
                throw new ArgumentNullException(nameof(shortUrl));
            }

            var request = await req.GetExpanderRequestAsync(shortUrl).ConfigureAwait(false);
            var headers = req.Headers.ToDictionary(p => p.Key, p => p.Value.Count == 1 ? (object) p.Value.First() : p.Value.ToList());
            var queries = req.Query.ToDictionary(p => p.Key, p => p.Value);

            this.ExpanderRequest = request;
            this.ExpanderRequestHeaders = headers;
            this.ExpanderRequestQueries = queries;

            return this;
        }

        /// <inheritdoc/>
        public async Task<IUrl> ValidateAsync()
        {
            if (this.ShortenerRequest == null)
            {
                throw new InvalidOperationException("request payload is not ready");
            }

            // Don't need when the friendly URL is not supplied.
            if (string.IsNullOrWhiteSpace(this.ShortenerRequest.Friendly))
            {
                this.IsFriendlyUrlValidated = true;

                return await Task.FromResult(this).ConfigureAwait(false);
            }

            var exists = await this._shortener.ExistsAsync(this.ShortenerRequest.Friendly).ConfigureAwait(false);
            if (exists)
            {
                throw new UrlExistsException("Friendly URL already exists") { ShortUrl = this.ShortenerRequest.Friendly };
            }

            this.IsFriendlyUrlValidated = true;

            return await Task.FromResult(this).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IUrl> ShortenAsync()
        {
            if (this.ShortenerRequest == null)
            {
                throw new InvalidOperationException("request payload is not ready");
            }

            var response = default(ShortenerResponse);

            var validated = false;
            while (!validated)
            {
                response = await this._shortener.ShortenAsync(this.ShortenerRequest).ConfigureAwait(false);

                // Only applies when the friendly URL is not supplied.
                if (!string.IsNullOrWhiteSpace(this.ShortenerRequest.Friendly))
                {
                    validated = true;
                    break;
                }

                var exists = await this._shortener.ExistsAsync(response.ShortUrl).ConfigureAwait(false);

                validated = !exists;
            }

            this.Shortened = response.Shortened;

            this.Original = response.Original;
            this.ShortUrl = response.ShortUrl;
            this.IsRandomUrlValidated = validated;
            this.Title = response.Title;
            this.Description = response.Description;
            this.Owner = response.Owner;
            this.CoOwners = response.CoOwners;
            this.HitCount = response.HitCount;

            this.ShortenerResponse = response;

            return this;
        }

        /// <inheritdoc/>
        public async Task<IUrl> ExpandAsync()
        {
            if (this.ExpanderRequest == null)
            {
                throw new InvalidOperationException("request payload is not ready");
            }

            var response = await this._expander.ExpandAsync(this.ExpanderRequest).ConfigureAwait(false);

            if (response != null)
            {
                this.Shortened = response.Shortened;
                this.UrlId = response.UrlId;

                this.EntityId = response.EntityId;
                this.Original = response.Original;
                this.ShortUrl = response.ShortUrl;
                this.Title = response.Title;
                this.Description = response.Description;
                this.Owner = response.Owner;
                this.CoOwners = response.CoOwners;
                this.HitCount = response.HitCount;
            }

            this.ExpanderResponse = response;

            if (response != null)
            {
                this.ExpanderResponse.RequestHeaders = this.ExpanderRequestHeaders;
                this.ExpanderResponse.RequestQueries = this.ExpanderRequestQueries;
            }

            return this;
        }

        /// <inheritdoc/>
        public async Task<IUrl> AddHitCountAsync<T>() where T : UrlResponse
        {
            if (typeof(T) == typeof(ShortenerResponse))
            {
                this.ShortenerResponse.HitCount++;
            }
            else if (typeof(T) == typeof(ExpanderResponse))
            {
                this.ExpanderResponse.HitCount++;
            }

            this.HitCount++;

            return await Task.FromResult(this).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IUrl> CreateRecordAsync(DateTimeOffset now, Guid entityId)
        {
            if (entityId == Guid.Empty)
            {
                throw new ArgumentException("Invalid entity ID");
            }

            var dateGenerated = now;
            var dateUpdated = now;

            this.ShortenerResponse.DateGenerated = dateGenerated;
            this.ShortenerResponse.DateUpdated = dateUpdated;
            this.ShortenerResponse.EntityId = entityId;

            await this._shortener.UpsertAsync(this.ShortenerResponse).ConfigureAwait(false);

            this.DateGenerated = dateGenerated;
            this.DateUpdated = dateUpdated;
            this.EntityId = entityId;

            return this;
        }

        /// <inheritdoc/>
        public async Task<IUrl> UpdateRecordAsync<T>(DateTimeOffset now, Guid? entityId = null) where T : UrlResponse
        {
            if (typeof(T) == typeof(ExpanderResponse) && !entityId.HasValue)
            {
                throw new ArgumentNullException(nameof(entityId));
            }

            if (typeof(T) == typeof(ExpanderResponse) && entityId.HasValue && entityId.Value == Guid.Empty)
            {
                throw new ArgumentException("Invalid entity ID");
            }

            var dateUpdated = now;

            if (typeof(T) == typeof(ShortenerResponse))
            {
                this.ShortenerResponse.DateUpdated = dateUpdated;

                await this._shortener.UpsertAsync(this.ShortenerResponse).ConfigureAwait(false);
            }
            else if (typeof(T) == typeof(ExpanderResponse))
            {
                this.ExpanderResponse.DateUpdated = dateUpdated;

                await this._expander.UpsertAsync<UrlItemEntity>(this.ExpanderResponse).ConfigureAwait(false);

                this.ExpanderResponse.EntityId = entityId.Value;

                await this._expander.UpsertAsync<VisitItemEntity>(this.ExpanderResponse).ConfigureAwait(false);
            }

            this.DateUpdated = dateUpdated;

            return this;
        }
    }
}

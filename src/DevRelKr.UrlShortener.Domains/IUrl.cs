using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Models.Requests;
using DevRelKr.UrlShortener.Models.Responses;

using Microsoft.AspNetCore.Http;

namespace DevRelKr.UrlShortener.Domains
{
    /// <summary>
    /// This provides interfaces to the <see cref="Url"/> class.
    /// </summary>
    public interface IUrl
    {
        /// <summary>
        /// Gets the entity ID.
        /// </summary>
        Guid EntityId { get; }

        /// <summary>
        /// Gets the original URL.
        /// </summary>
        Uri Original { get; }

        /// <summary>
        /// Gets the shortened URL.
        /// </summary>
        Uri Shortened { get; }

        /// <summary>
        /// Gets the short URL value.
        /// </summary>
        string ShortUrl { get; }

        /// <summary>
        /// Gets the value indicating whether the friendly URL was validated or not.
        /// </summary>
        bool IsFriendlyUrlValidated { get; }

        /// <summary>
        /// Gets the value indicating whether the random URL was validated or not.
        /// </summary>
        bool IsRandomUrlValidated { get; }

        /// <summary>
        /// Gets the title of the shortened URL.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the description of the shortened URL.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the owner.
        /// </summary>
        string Owner { get; }

        /// <summary>
        /// Gets the list of co-owners.
        /// </summary>
        List<string> CoOwners { get; }

        /// <summary>
        /// Gets the hit count.
        /// </summary>
        int HitCount { get; }

        /// <summary>
        /// Gets the date/time when the shortened URL was generated.
        /// </summary>
        DateTimeOffset DateGenerated { get; }

        /// <summary>
        /// Gets the date/time when the shortened URL was updated.
        /// </summary>
        DateTimeOffset DateUpdated { get; }

        /// <summary>
        /// Gets the <see cref="Models.Requests.ShortenerRequest"/> instance.
        /// </summary>
        ShortenerRequest ShortenerRequest { get; }

        /// <summary>
        /// Gets the <see cref="Models.Responses.ShortenerResponse"/> instance.
        /// </summary>
        ShortenerResponse ShortenerResponse { get; }

        /// <summary>
        /// Gets the <see cref="Models.Requests.ExpanderRequest"/> instance.
        /// </summary>
        ExpanderRequest ExpanderRequest { get; }

        /// <summary>
        /// Gets the <see cref="Dictionary{string, object}"/> instance as the expander request header values.
        /// </summary>
        Dictionary<string, object> ExpanderRequestHeaders { get; }

        /// <summary>
        /// Gets the <see cref="Models.Responses.ExpanderResponse"/> instance.
        /// </summary>
        ExpanderResponse ExpanderResponse { get; }

        /// <summary>
        /// Gets the request for URL shortening.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> instance.</param>
        /// <returns>Returns the <see cref="Task{IUrl}"/> instance.</returns>
        Task<IUrl> GetRequestAsync(HttpRequest req);

        /// <summary>
        /// Gets the request for URL expanding.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> instance.</param>
        /// <param name="shortUrl">Short URL value.</param>
        /// <returns>Returns the <see cref="Task{IUrl}"/> instance.</returns>
        Task<IUrl> GetRequestAsync(HttpRequest req, string shortUrl);

        /// <summary>
        /// Validates whether the given friendly URL already exists or not. If exists, it throws an exception.
        /// </summary>
        /// <returns>Returns the <see cref="Task{IUrl}"/> instance.</returns>
        Task<IUrl> ValidateAsync();

        /// <summary>
        /// Shortens the URL.
        /// </summary>
        /// <returns>Returns the <see cref="Task{IUrl}"/> instance.</returns>
        Task<IUrl> ShortenAsync();

        /// <summary>
        /// Expands the shortened URL.
        /// </summary>
        /// <returns>Returns the <see cref="Task{IUrl}"/> instance.</returns>
        Task<IUrl> ExpandAsync();

        /// <summary>
        /// Adds hit count of the URL.
        /// </summary>
        /// <typeparam name="T">Type of the response.</typeparam>
        /// <returns>Returns the <see cref="Task{IUrl}"/> instance.</returns>
        Task<IUrl> AddHitCountAsync<T>() where T : UrlResponse;

        /// <summary>
        /// Adds the record to data store.
        /// </summary>
        /// <param name="now"><see cref="DateTimeOffset"/> value.</param>
        /// <param name="entityId"><see cref="Guid"/> value as the ID.</param>
        /// <returns>Returns the <see cref="Task{IUrl}"/> instance.</returns>
        Task<IUrl> CreateRecordAsync(DateTimeOffset now, Guid entityId);

        /// <summary>
        /// Updates the record to data store.
        /// </summary>
        /// <typeparam name="T">Type of the response.</typeparam>
        /// <param name="now"><see cref="DateTimeOffset"/> value.</param>
        /// <returns>Returns the <see cref="Task{IUrl}"/> instance.</returns>
        Task<IUrl> UpdateRecordAsync<T>(DateTimeOffset now) where T : UrlResponse;
    }
}

using System;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Models.Requests;
using DevRelKr.UrlShortener.Models.Responses;

namespace DevRelKr.UrlShortener.Services
{
    /// <summary>
    /// This provides interfaces to the <see cref="ShortenerService"/> class.
    /// </summary>
    public interface IShortenerService
    {
        /// <summary>
        /// Shortens the given URL.
        /// </summary>
        /// <param name="payload"><see cref="ShortenerRequest"/> instance.</param>
        /// <returns>Returns the <see cref="Task{ShortenerResponse}"/> instance.</returns>
        Task<ShortenerResponse> ShortenAsync(ShortenerRequest payload);

        /// <summary>
        /// Checks whether the short URL already exists or not.
        /// </summary>
        /// <param name="shortUrl">Short URL value.</param>
        /// <returns>Returns <c>True</c>, if the short URL already exists; otherwise returns <c>False</c>.</returns>
        Task<bool> ExistsAsync(string shortUrl);

        /// <summary>
        /// Adds or updates the short URL record.
        /// </summary>
        /// <param name="payload"><see cref="ShortenerResponse"/> object.</param>
        /// <returns>Returns the HTTP status code of this transaction.</returns>
        Task<int> UpsertAsync(ShortenerResponse payload);
    }
}

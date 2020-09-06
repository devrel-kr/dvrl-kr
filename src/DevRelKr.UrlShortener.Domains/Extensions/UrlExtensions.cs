using System;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Models.Responses;

namespace DevRelKr.UrlShortener.Domains.Extensions
{
    /// <summary>
    /// This represents the extension entity for <ses cref="IUrl"/>.
    /// </summary>
    public static class UrlExtensions
    {
        /// <summary>
        /// Validates whether the given friendly URL already exists or not. If exists, it throws an exception.
        /// </summary>
        /// <param name="value"><see cref="Task{IUrl}"/> instance.</param>
        /// <returns>Returns the <see cref="Task{IUrl}"/> instance.</returns>
        public static async Task<IUrl> ValidateAsync(this Task<IUrl> value)
        {
            var instance = await value.ConfigureAwait(false);

            return await instance.ValidateAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Shortens the URL.
        /// </summary>
        /// <param name="value"><see cref="Task{IUrl}"/> instance.</param>
        /// <returns>Returns the <see cref="Task{IUrl}"/> instance.</returns>
        public static async Task<IUrl> ShortenAsync(this Task<IUrl> value)
        {
            var instance = await value.ConfigureAwait(false);

            return await instance.ShortenAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Expands the shortened URL.
        /// </summary>
        /// <param name="value"><see cref="Task{IUrl}"/> instance.</param>
        /// <returns>Returns the <see cref="Task{IUrl}"/> instance.</returns>
        public static async Task<IUrl> ExpandAsync(this Task<IUrl> value)
        {
            var instance = await value.ConfigureAwait(false);

            return await instance.ExpandAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Adds the hit count.
        /// </summary>
        /// <typeparam name="T">Type of the response.</typeparam>
        /// <param name="value"><see cref="Task{IUrl}"/> instance.</param>
        /// <returns>Returns the <see cref="Task{IUrl}"/> instance.</returns>
        public static async Task<IUrl> AddHitCountAsync<T>(this Task<IUrl> value) where T : UrlResponse
        {
            var instance = await value.ConfigureAwait(false);

            return await instance.AddHitCountAsync<T>().ConfigureAwait(false);
        }

        /// <summary>
        /// Adds the record to data store.
        /// </summary>
        /// <param name="value"><see cref="Task{IUrl}"/> instance.</param>
        /// <param name="now"><see cref="DateTimeOffset"/> value.</param>
        /// <param name="entityId"><see cref="Guid"/> value as the ID.</param>
        /// <returns>Returns the <see cref="Task{IUrl}"/> instance.</returns>
        public static async Task<IUrl> CreateRecordAsync(this Task<IUrl> value, DateTimeOffset now, Guid entityId)
        {
            var instance = await value.ConfigureAwait(false);

            return await instance.CreateRecordAsync(now, entityId).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the record to data store.
        /// </summary>
        /// <typeparam name="T">Type of the response.</typeparam>
        /// <param name="value"><see cref="Task{IUrl}"/> instance.</param>
        /// <param name="now"><see cref="DateTimeOffset"/> value.</param>
        /// <returns>Returns the <see cref="Task{IUrl}"/> instance.</returns>
        public static async Task<IUrl> UpdateRecordAsync<T>(this Task<IUrl> value, DateTimeOffset now) where T : UrlResponse
        {
            var instance = await value.ConfigureAwait(false);

            return await instance.UpdateRecordAsync<T>(now).ConfigureAwait(false);
        }
    }
}

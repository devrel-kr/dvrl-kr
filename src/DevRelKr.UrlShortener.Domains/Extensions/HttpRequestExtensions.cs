using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Models.Requests;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

namespace DevRelKr.UrlShortener.Domains.Extensions
{
    /// <summary>
    /// This represents the extensions entity for <see cref="HttpRequest"/>.
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Extracts <see cref="ShortenerRequest"/> from <see cref="HttpRequest"/>.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> instance.</param>
        /// <returns>Returns <see cref="ShortenerRequest"/> instance.</returns>
        public static async Task<ShortenerRequest> GetShortenerRequestAsync(this HttpRequest req)
        {
            if (req == null)
            {
                throw new ArgumentNullException(nameof(req));
            }

            var request = default(ShortenerRequest);
            switch (req.Method.ToLowerInvariant())
            {
                case "get":
                    request = await GetShortenerRequestFromQueryAsync(req.Query).ConfigureAwait(false);
                    break;

                case "post":
                    request = await GetShortenerRequestFromBodyAsync(req.Body).ConfigureAwait(false);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return request;
        }

        /// <summary>
        /// Extracts <see cref="ExpanderRequest"/> from the given value.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> instance.</param>
        /// <param name="shortUrl">Short URL value.</param>
        /// <returns>Returns <see cref="ExpanderRequest"/> instance.</returns>
        public static async Task<ExpanderRequest> GetExpanderRequestAsync(this HttpRequest req, string shortUrl)
        {
            if (req == null)
            {
                throw new ArgumentNullException(nameof(req));
            }

            if (string.IsNullOrWhiteSpace(shortUrl))
            {
                throw new ArgumentNullException(nameof(shortUrl));
            }

            if (req.Method.ToLowerInvariant() != "get")
            {
                throw new InvalidOperationException();
            }

            var request = new ExpanderRequest()
            {
                ShortUrl = shortUrl
            };

            return await Task.FromResult(request).ConfigureAwait(false);
        }

        private static async Task<ShortenerRequest> GetShortenerRequestFromQueryAsync(IQueryCollection query)
        {
            var original = query["original"];
            if (!original.Any())
            {
                throw new InvalidOperationException("Original URL is missing");
            }

            var owner = query["owner"];
            if (!owner.Any())
            {
                throw new InvalidOperationException("Owner is missing");
            }

            var friendly = query["friendly"];
            var title = query["title"];
            var description = query["description"];
            var coOwners = query["coowners"];

            var request = new ShortenerRequest()
            {
                Original = new Uri(original),
                Friendly = friendly,
                Title = title,
                Description = description,
                Owner = owner,
                CoOwners = coOwners.ToList()
            };

            return await Task.FromResult(request).ConfigureAwait(false);
        }

        private static async Task<ShortenerRequest> GetShortenerRequestFromBodyAsync(Stream body)
        {
            using (var reader = new StreamReader(body))
            {
                var payload = await reader.ReadToEndAsync();
                var request = JsonConvert.DeserializeObject<ShortenerRequest>(payload);

                return request;
            }
        }
    }
}

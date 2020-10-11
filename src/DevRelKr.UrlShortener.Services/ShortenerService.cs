using System;
using System.Text;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Models.Configurations;
using DevRelKr.UrlShortener.Models.DataStores;
using DevRelKr.UrlShortener.Models.Requests;
using DevRelKr.UrlShortener.Models.Responses;
using DevRelKr.UrlShortener.Repositories;

namespace DevRelKr.UrlShortener.Services
{
    /// <summary>
    /// This represents the service entity for the URL shortener.
    /// </summary>
    public class ShortenerService : IShortenerService
    {
        private const string CharacterPool = "abcdefghijklmnopqrstuvwxyz0123456789";

        private static int characterPoolLength = CharacterPool.Length;
        private static Random random = new Random();

        private readonly AppSettings _settings;
        private readonly IQuery _query;
        private readonly ICommand _command;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortenerService"/> class.
        /// </summary>
        /// <param name="settings"><see cref="AppSettings"/> instance.</param>
        /// <param name="query"><see cref="IQuery"/> instance.</param>
        /// <param name="command"><see cref="ICommand"/> instance.</param>
        public ShortenerService(AppSettings settings, IQuery query, ICommand command)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this._query = query ?? throw new ArgumentNullException(nameof(query));
            this._command = command ?? throw new ArgumentNullException(nameof(command));
        }

        /// <inheritdoc/>
        public async Task<ShortenerResponse> ShortenAsync(ShortenerRequest payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var response = new ShortenerResponse()
            {
                Original = payload.Original,
                Title = payload.Title,
                Description = payload.Description,
                Owner = payload.Owner,
                CoOwners = payload.CoOwners,
            };

            if (!string.IsNullOrWhiteSpace(payload.Friendly))
            {
                response.Shortened = new Uri($"https://{this._settings.ShortenUrl.Hostname}/{payload.Friendly}");
                response.ShortUrl = payload.Friendly;

                return await Task.FromResult(response).ConfigureAwait(false);
            }

            var sb = new StringBuilder();
            for(var i = 0; i < this._settings.ShortenUrl.Length; i++)
            {
                var index = random.Next(this._settings.ShortenUrl.Length);
                sb.Append(CharacterPool[index]);
            }

            response.Shortened = new Uri($"https://{this._settings.ShortenUrl.Hostname}/{sb.ToString()}");
            response.ShortUrl = sb.ToString();

            return await Task.FromResult(response).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(string shortUrl)
        {
            if (string.IsNullOrWhiteSpace(shortUrl))
            {
                throw new ArgumentNullException(nameof(shortUrl));
            }

            var item = await this._query.GetUrlItemEntityAsync(shortUrl);

            return item != null;
        }

        /// <inheritdoc/>
        public async Task<int> UpsertAsync(ShortenerResponse payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var item = new UrlItemEntity(payload);

            var result = await this._command.UpsertItemEntityAsync<UrlItemEntity>(item).ConfigureAwait(false);

            return result;
        }
    }
}

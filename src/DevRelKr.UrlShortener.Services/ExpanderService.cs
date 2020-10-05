using System;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Models.Configurations;
using DevRelKr.UrlShortener.Models.DataStores;
using DevRelKr.UrlShortener.Models.Requests;
using DevRelKr.UrlShortener.Models.Responses;
using DevRelKr.UrlShortener.Repositories;

namespace DevRelKr.UrlShortener.Services
{
    /// <summary>
    /// This represents the service entity for the URL expander.
    /// </summary>
    public class ExpanderService : IExpanderService
    {
        private readonly AppSettings _settings;
        private readonly IQuery _query;
        private readonly ICommand _command;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpanderService"/> class.
        /// </summary>
        /// <param name="settings"><see cref="AppSettings"/> instance.</param>
        /// <param name="query"><see cref="IQuery"/> instance.</param>
        /// <param name="command"><see cref="ICommand"/> instance.</param>
        public ExpanderService(AppSettings settings, IQuery query, ICommand command)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this._query = query ?? throw new ArgumentNullException(nameof(query));
            this._command = command ?? throw new ArgumentNullException(nameof(command));
        }

        /// <inheritdoc/>
        public async Task<ExpanderResponse> ExpandAsync(ExpanderRequest payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var item = await this._query.GetUrlItemEntityAsync(payload.ShortUrl);
            if (item == null)
            {
                return null;
            }

            var response = new ExpanderResponse(item);
            response.Shortened = new Uri($"https://{this._settings.ShortenUrl.Hostname.TrimEnd('/')}/{item.ShortUrl}");

            return response;
        }

        /// <inheritdoc/>
        public async Task<int> UpsertAsync<T>(ExpanderResponse payload) where T : ItemEntity
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var item = (T) Activator.CreateInstance(typeof(T), payload);

            var result = await this._command.UpsertItemEntityAsync<T>(item).ConfigureAwait(false);

            return result;
        }
    }
}

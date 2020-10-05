using System;
using System.Text;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Models.DataStores;

using Microsoft.Azure.Cosmos;

namespace DevRelKr.UrlShortener.Repositories
{
    /// <summary>
    /// This represents the query entity for Cosmos DB.
    /// </summary>
    public class CosmosDbQuery : IQuery
    {
        private readonly ICosmosDbContainerHelper _helper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosDbQuery"/> class.
        /// </summary>
        /// <param name="helper"><see cref="ICosmosDbContainerHelper"/> instance.</param>
        public CosmosDbQuery(ICosmosDbContainerHelper helper)
        {
            this._helper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        /// <inheritdoc/>
        public async Task<UrlItemEntity> GetUrlItemEntityAsync(string shortUrl)
        {
            if (string.IsNullOrWhiteSpace(shortUrl))
            {
                throw new ArgumentNullException(nameof(shortUrl));
            }

            var container = await this._helper.GetContainerAsync().ConfigureAwait(false);

            var qb = new StringBuilder()
                         .Append("SELECT * FROM dvrl d WHERE d.collection = 'Url' AND d.shortUrl = @shortUrl");
            var definition = new QueryDefinition(qb.ToString())
                                 .WithParameter("@shortUrl", shortUrl);
            var options = new QueryRequestOptions() { MaxItemCount = 1 };

            var entity = default(UrlItemEntity);
            using (var iterator = container.GetItemQueryIterator<UrlItemEntity>(definition, requestOptions: options))
            {
                while (iterator.HasMoreResults)
                {
                    // Implicit operation is not testable
                    // foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                    // {
                    //     entity = item;
                    // }

                    var feed = await iterator.ReadNextAsync().ConfigureAwait(false);

                    // Implicit operation is not testable
                    // foreach (var item in feed)
                    // {
                    //     entity = item;
                    // }

                    foreach (var item in feed.Resource)
                    {
                        entity = item;
                    }
                }
            }

            return entity;
        }

        /// <inheritdoc/>
        public async Task<UrlItemEntityCollection> GetUrlItemEntityCollectionAsync(string owner)
        {
            if (string.IsNullOrWhiteSpace(owner))
            {
                throw new ArgumentNullException(nameof(owner));
            }

            var container = await this._helper.GetContainerAsync().ConfigureAwait(false);

            var qb = new StringBuilder()
                         .Append("SELECT * FROM dvrl d WHERE d.collection = 'Url' AND d.owner = @owner");
            var definition = new QueryDefinition(qb.ToString())
                                 .WithParameter("@owner", owner);

            var collection = new UrlItemEntityCollection();
            using (var iterator = container.GetItemQueryIterator<UrlItemEntity>(definition))
            {
                while (iterator.HasMoreResults)
                {
                    foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                    {
                        collection.Items.Add(item);
                    }
                }
            }

            return collection;
        }

        /// <inheritdoc/>
        public async Task<VisitItemEntityCollection> GetVisitItemEntityCollectionAsync(string shortUrl)
        {
            if (string.IsNullOrWhiteSpace(shortUrl))
            {
                throw new ArgumentNullException(nameof(shortUrl));
            }

            var container = await this._helper.GetContainerAsync().ConfigureAwait(false);

            var qb = new StringBuilder()
                         .Append("SELECT * FROM dvrl d WHERE d.collection = 'Visit' AND d.shortUrl = @shortUrl");
            var definition = new QueryDefinition(qb.ToString())
                                 .WithParameter("@shortUrl", shortUrl);

            var collection = new VisitItemEntityCollection();
            using (var iterator = container.GetItemQueryIterator<VisitItemEntity>(definition))
            {
                while (iterator.HasMoreResults)
                {
                    foreach (var item in await iterator.ReadNextAsync().ConfigureAwait(false))
                    {
                        collection.Items.Add(item);
                    }
                }
            }

            return collection;
        }
    }
}

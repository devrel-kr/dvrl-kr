using System;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Models.DataStores;

using Microsoft.Azure.Cosmos;

namespace DevRelKr.UrlShortener.Repositories
{
    public class CosmosDbCommand : ICommand
    {
        private readonly ICosmosDbContainerHelper _helper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosDbCommand"/> class.
        /// </summary>
        /// <param name="helper"><see cref="ICosmosDbContainerHelper"/> instance.</param>
        public CosmosDbCommand(ICosmosDbContainerHelper helper)
        {
            this._helper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        /// <inheritdoc/>
        public async Task<int> UpsertUrlItemEntityAsync(UrlItemEntity item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var container = await this._helper.GetContainerAsync().ConfigureAwait(false);

            var result = await container.UpsertItemAsync<UrlItemEntity>(item, new PartitionKey(item.PartitionKey));

            return (int) result.StatusCode;
        }
    }
}

using System;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Models.Configurations;

using Microsoft.Azure.Cosmos;

namespace DevRelKr.UrlShortener.Repositories
{
    /// <summary>
    /// This represents the helper entity for Cosmos DB containers.
    /// </summary>
    public class CosmosDbContainerHelper : ICosmosDbContainerHelper
    {
        private readonly AppSettings _settings;
        private readonly CosmosClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosDbContainerHelper"/> class.
        /// </summary>
        /// <param name="settings"><see cref="AppSettings"/> instance.</param>
        /// <param name="client"><see cref="CosmosClient"/> instance.</param>
        public CosmosDbContainerHelper(AppSettings settings, CosmosClient client)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this._client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <inheritdoc/>
        public async Task<Container> GetContainerAsync()
        {
            var databaseName = this._settings.CosmosDb.DatabaseName;
            var containerName = this._settings.CosmosDb.ContainerName;
            var partitionKeyPath = this._settings.CosmosDb.PartitionKeyPath;

            var db = (Database) await this._client
                                          .CreateDatabaseIfNotExistsAsync(databaseName)
                                          .ConfigureAwait(false);

            var properties = new ContainerProperties(containerName, partitionKeyPath);
            var container = (Container) await db.CreateContainerIfNotExistsAsync(properties)
                                                .ConfigureAwait(false);

            return container;
        }
    }
}

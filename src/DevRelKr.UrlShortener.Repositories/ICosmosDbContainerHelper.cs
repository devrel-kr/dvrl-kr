using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;

namespace DevRelKr.UrlShortener.Repositories
{
    /// <summary>
    /// This provides interfaces to the <see cref="CosmosDbContainerHelper"/> class.
    /// </summary>
    public interface ICosmosDbContainerHelper
    {
        /// <summary>
        /// Gets the Cosmos DB container instance.
        /// </summary>
        /// <returns>Returns the <see cref="Task{Container}"/> instance.</returns>
        Task<Container> GetContainerAsync();
    }
}

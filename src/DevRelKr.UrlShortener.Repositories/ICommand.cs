using System.Threading.Tasks;

using DevRelKr.UrlShortener.Models.DataStores;

namespace DevRelKr.UrlShortener.Repositories
{
    /// <summary>
    /// This provides interfaces to the classes that implementing this interface.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Adds or updates the <see cref="UrlItemEntity"/> object.
        /// </summary>
        /// <param name="item"><see cref="UrlItemEntity"/> object.</param>
        /// <returns>Returns the HTTP status code indicating whether the transaction is of success or failure.</returns>
        Task<int> UpsertUrlItemEntityAsync(UrlItemEntity item);
    }
}

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
        /// Adds or updates the <see cref="ItemEntity"/> object.
        /// </summary>
        /// <typeparam name="T">Type of entity represented as <see cref="ItemEntity"/>.</typeparam>
        /// <param name="item"><see cref="ItemEntity"/> object.</param>
        /// <returns>Returns the HTTP status code indicating whether the transaction is of success or failure.</returns>
        Task<int> UpsertItemEntityAsync<T>(T item) where T : ItemEntity;
    }
}

using System.Threading.Tasks;

using DevRelKr.UrlShortener.Models.DataStores;

namespace DevRelKr.UrlShortener.Repositories
{
    /// <summary>
    /// This provides interfaces to the classes that implementing this interface.
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// Gets the <see cref="UrlItemEntity"/> object.
        /// </summary>
        /// <param name="shortUrl">Short URL value.</param>
        /// <returns>Returns the <see cref="Task{UrlItemEntity}"/> object.</returns>
        Task<UrlItemEntity> GetUrlItemEntityAsync(string shortUrl);

        /// <summary>
        /// Gets the <see cref="UrlItemEntityCollection"/> object filtered by owner.
        /// </summary>
        /// <param name="owner">Short URL owner value.</param>
        /// <returns>Returns the <see cref="Task{UrlItemEntityCollection}"/> object.</returns>
        Task<UrlItemEntityCollection> GetUrlItemEntityCollectionAsync(string owner);

        /// <summary>
        /// Gets the <see cref="VisitItemEntityCollection"/> object filtered by short URL.
        /// </summary>
        /// <param name="owner">Short URL owner value.</param>
        /// <returns>Returns the <see cref="Task{VisitUrlItemEntityCollection}"/> object.</returns>
        Task<VisitItemEntityCollection> GetVisitItemEntityCollectionAsync(string shortUrl);
    }
}

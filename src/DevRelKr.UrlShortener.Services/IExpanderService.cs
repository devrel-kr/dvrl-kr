using System.Threading.Tasks;

using DevRelKr.UrlShortener.Models.DataStores;
using DevRelKr.UrlShortener.Models.Requests;
using DevRelKr.UrlShortener.Models.Responses;

namespace DevRelKr.UrlShortener.Services
{
    /// <summary>
    /// This provides interfaces to the <see cref="ExpanderService"/> class.
    /// </summary>
    public interface IExpanderService
    {
        /// <summary>
        /// Expands the given URL.
        /// </summary>
        /// <param name="payload"><see cref="ExpanderRequest"/> instance.</param>
        /// <returns>Returns the <see cref="Task{ExpanderResponse}"/> instance.</returns>
        Task<ExpanderResponse> ExpandAsync(ExpanderRequest payload);

        /// <summary>
        /// Adds or updates the URL record.
        /// </summary>
        /// <typeparam name="T">Type of entity represented as <see cref="ItemEntity"/>.</typeparam>
        /// <param name="payload"><see cref="ExpanderResponse"/> object.</param>
        /// <returns>Returns the HTTP status code of this transaction.</returns>
        Task<int> UpsertAsync<T>(ExpanderResponse payload) where T : ItemEntity;
    }
}

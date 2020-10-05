using System.Collections.Generic;

namespace DevRelKr.UrlShortener.Models.DataStores
{
    /// <summary>
    /// This represents the collection entity of the <see cref="ItemEntity"/> object.
    /// </summary>
    /// <typeparam name="T">Type of entity represented as <see cref="ItemEntity"/>.</typeparam>
    public abstract class ItemEntityCollection<T> where T : ItemEntity
    {
        /// <summary>
        /// Gets or sets the list of the items that can be represented as <see cref="ItemEntity"/>.
        /// </summary>
        public virtual List<T> Items { get; set;} = new List<T>();
    }
}

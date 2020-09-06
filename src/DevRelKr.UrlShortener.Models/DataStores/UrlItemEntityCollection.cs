using System.Collections.Generic;

namespace DevRelKr.UrlShortener.Models.DataStores
{
    /// <summary>
    /// This represents the collection entity of the <see cref="UrlItemEntity"/> object.
    /// </summary>
    public class UrlItemEntityCollection
    {
        /// <summary>
        /// Gets or sets the list of the <see cref="UrlItemEntity"/> objects.
        /// </summary>
        public virtual List<UrlItemEntity> Items { get; set;} = new List<UrlItemEntity>();
    }
}

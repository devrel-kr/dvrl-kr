using System;

using Newtonsoft.Json;

namespace DevRelKr.UrlShortener.Models.DataStores
{
    /// <summary>
    /// This represents the item entity. This MUST be inherited.
    /// </summary>
    public abstract class ItemEntity
    {
        private Guid _entityId;
        private PartitionType _collection;
        private DateTimeOffset _dateGenerated;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemEntity"/> class.
        /// </summary>
        protected ItemEntity()
        {
        }

        /// <summary>
        /// Gets or sets the entity ID.
        /// </summary>
        [JsonRequired]
        [JsonProperty("id", Order = int.MinValue)]
        public virtual Guid EntityId
        {
            get { return this._entityId; }
            set
            {
                if (value == Guid.Empty)
                {
                    throw new InvalidOperationException("GUID: Value not allowed");
                }

                this._entityId = value;
            }
        }

        /// <summary>
        /// Gets or sets the collection as a partition.
        /// </summary>
        [JsonRequired]
        [JsonProperty("collection", Order = int.MinValue)]
        public virtual PartitionType Collection
        {
            get { return this._collection; }
            set
            {
                if (value == PartitionType.None)
                {
                    throw new InvalidOperationException("Collection: Value not allowed");
                }

                this._collection = value;
            }
        }

        /// <summary>
        /// Gets or sets the date/time when the shortened URL was generated.
        /// </summary>
        [JsonRequired]
        [JsonProperty("dateGenerated", Order = int.MaxValue)]
        public virtual DateTimeOffset DateGenerated
        {
            get { return this._dateGenerated; }
            set
            {
                if (value == DateTimeOffset.MinValue)
                {
                    throw new InvalidOperationException("DateGenerated: Value not allowed");
                }

                this._dateGenerated = value;
            }
        }

        /// <summary>
        /// Gets the partition key.
        /// </summary>
        [JsonIgnore]
        public virtual string PartitionKey => this.Collection.ToString();

        /// <summary>
        /// Gets the partition key path.
        /// </summary>
        [JsonIgnore]
        public virtual string PartitionKeyPath => "/collection";
    }
}

namespace DevRelKr.UrlShortener.Models.Configurations
{
    /// <summary>
    /// This represents the app settings for Cosmos DB.
    /// </summary>
    public class CosmosDbSettings
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public virtual string ConnectionString { get; set; }

        public virtual string DatabaseName { get; set; }

        public virtual string ContainerName { get; set; }

        public virtual string PartitionKeyPath { get; set; }
    }
}

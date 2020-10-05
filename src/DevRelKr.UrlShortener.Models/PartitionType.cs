using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DevRelKr.UrlShortener.Models
{
    /// <summary>
    /// This specifies the type of the record as a partition key
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PartitionType
    {
        /// <summary>
        /// Identifies nothing.
        /// </summary>
        None = 0,

        /// <summary>
        /// Identifies the URL.
        /// </summary>
        Url = 1,

        /// <summary>
        /// Identifies the Visit.
        /// </summary>
        Visit = 2
    }
}

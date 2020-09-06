namespace DevRelKr.UrlShortener.Models.Configurations
{
    /// <summary>
    /// This represents the app settings entity for the shorten URLs.
    /// </summary>
    public class ShortenUrlSettings
    {
        /// <summary>
        /// Gets or sets the length of the shortened URLs.
        /// </summary>
        public virtual int Length { get; set;}

        /// <summary>
        /// Gets or sets the hostname.
        /// </summary>
        public virtual string Hostname { get; set; }
    }
}

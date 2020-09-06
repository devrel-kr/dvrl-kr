namespace DevRelKr.UrlShortener.Models
{
    /// <summary>
    /// This specifies the URL handling mode.
    /// </summary>
    public enum UrlMode
    {
        /// <summary>
        /// Identifies the URL to be shortened.
        /// </summary>
        Shorten = 1,

        /// <summary>
        /// Identifies the URL to be expanded.
        /// </summary>
        Expand = 2
    }
}

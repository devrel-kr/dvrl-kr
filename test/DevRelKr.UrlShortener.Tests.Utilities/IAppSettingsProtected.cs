using Microsoft.Extensions.Configuration;

namespace DevRelKr.UrlShortener.Tests.Utilities
{
    /// <summary>
    /// This provides interfaces for <see cref="AppSettings"/> mocking.
    /// </summary>
    public interface IAppSettingsProtected
    {
        /// <summary>
        /// Gets the <see cref="IConfiguration"/> instance.
        /// </summary>
        /// <value></value>
        IConfiguration Config { get; }
    }
}

using Aliencube.AzureFunctions.Extensions.Configuration.AppSettings.Resolvers;

using DevRelKr.UrlShortener.Models.Configurations;

using Moq;
using Moq.Protected;

namespace DevRelKr.UrlShortener.Tests.Utilities
{
    /// <summary>
    /// This represents the mocker entity for the <see cref="AppSettingsMocker"/> class.
    /// </summary>
    public class AppSettingsMocker
    {
        /// <summary>
        /// Creates mocked instance of the <see cref="AppSettings"/> class.
        /// </summary>
        /// <returns>Returns the <see cref="Mock{AppSettings}"/> instance.</returns>
        public Mock<AppSettings> CreateAppSettingsInstance()
        {
            var config = ConfigurationResolver.Resolve();

            var settings = new Mock<AppSettings>();
            settings.Protected()
                    .As<IAppSettingsProtected>()
                    .Setup(p => p.Config)
                    .Returns(config);

            return settings;
        }
    }
}

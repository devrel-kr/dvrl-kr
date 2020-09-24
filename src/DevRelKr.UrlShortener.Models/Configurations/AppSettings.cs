using System.Collections.Generic;
using System.Linq;

using Aliencube.AzureFunctions.Extensions.Configuration.AppSettings;
using Aliencube.AzureFunctions.Extensions.Configuration.AppSettings.Extensions;

using Microsoft.Extensions.Configuration;

namespace DevRelKr.UrlShortener.Models.Configurations
{
    /// <summary>
    /// This represents the app settings entity.
    /// </summary>
    public class AppSettings : AppSettingsBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettings"/> class.
        /// </summary>
        public AppSettings()
            : base()
        {
            this.IsProduction = this.Config.GetValue<string>("AZURE_FUNCTIONS_ENVIRONMENT") == "Production";
            this.FilesToBeIgnired = this.Config.GetValue<string>("FilesToBeIgnored")
                                               .Split(new[] { "," }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
            this.GoogleAnalyticsCode = this.Config.GetValue<string>("GoogleAnalyticsCode");
            this.ShortenUrl = this.Config.Get<ShortenUrlSettings>("ShortenUrl");
            this.CosmosDb = this.Config.Get<CosmosDbSettings>("CosmosDb");
            this.CosmosDb.ConnectionString = this.Config.GetValue<string>("CosmosDBConnection");
        }

        /// <summary>
        /// Gets the value indicating whether the runtime is in production or not.
        /// </summary>
        public virtual bool IsProduction { get; }

        /// <summary>
        /// Gets the list of files to be ignored from rendering.
        /// </summary>
        public virtual List<string> FilesToBeIgnired { get; }

        /// <summary>
        /// Gets the Google Analytics code.
        /// </summary>
        public virtual string GoogleAnalyticsCode { get; }

        /// <summary>
        /// Gets the <see cref="ShortenUrlSettings"/> instance.
        /// </summary>
        public virtual ShortenUrlSettings ShortenUrl { get; }

        /// <summary>
        /// Gets the <see cref="CosmosDbSettings"/> instance.
        /// </summary>
        public virtual CosmosDbSettings CosmosDb { get; }
    }
}

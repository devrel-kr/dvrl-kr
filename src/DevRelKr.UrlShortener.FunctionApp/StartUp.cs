using DevRelKr.UrlShortener.Domains;
using DevRelKr.UrlShortener.Models.Configurations;
using DevRelKr.UrlShortener.Repositories;
using DevRelKr.UrlShortener.Services;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(DevRelKr.UrlShortener.FunctionApp.StartUp))]
namespace DevRelKr.UrlShortener.FunctionApp
{
    /// <summary>
    /// This represents the entity to be invoked during the runtime startup.
    /// </summary>
    public class StartUp : FunctionsStartup
    {
        /// <inheritdoc />
        public override void Configure(IFunctionsHostBuilder builder)
        {
            this.ConfigureAppSettings(builder.Services);
            this.ConfigureClients(builder.Services);
            this.ConfigureHelpers(builder.Services);
            this.ConfigureRepositories(builder.Services);
            this.ConfigureServices(builder.Services);
            this.ConfigureDomains(builder.Services);
        }

        private void ConfigureAppSettings(IServiceCollection services)
        {
            services.AddSingleton<AppSettings>();
        }

        private void ConfigureClients(IServiceCollection services)
        {
            var settings = services.BuildServiceProvider()
                                   .GetService<AppSettings>();

            var client = new CosmosClientBuilder(settings.CosmosDb.ConnectionString)
                             .Build();

            services.AddSingleton<CosmosClient>(client);
        }

        private void ConfigureHelpers(IServiceCollection services)
        {
            services.AddSingleton<ICosmosDbContainerHelper, CosmosDbContainerHelper>();
        }

        private void ConfigureRepositories(IServiceCollection services)
        {
            services.AddSingleton<IQuery, CosmosDbQuery>();
            services.AddSingleton<ICommand, CosmosDbCommand>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IShortenerService, ShortenerService>();
            services.AddTransient<IExpanderService, ExpanderService>();
        }

        private void ConfigureDomains(IServiceCollection services)
        {
            services.AddTransient<IUrl, Url>();
        }
    }
}

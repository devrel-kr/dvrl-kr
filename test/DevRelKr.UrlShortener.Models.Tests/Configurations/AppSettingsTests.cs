using System;

using DevRelKr.UrlShortener.Models.Configurations;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevRelKr.UrlShortener.Models.Tests.Configurations
{
    [TestClass]
    public class AppSettingsTests
    {
        [DataTestMethod]
        [DataRow("Development", "favicon.ico", "UA-123456-7", "dvrl.kr", 10, "abcdefg", "database", "container", "partitionkeypath")]
        public void Given_ShortenUrlSettings_When_Initiated_Then_It_Should_Return_Result(string environment, string filesToBeIgnored, string gaCode, string hostname, int length, string connectionString, string databaseName, string containerName, string partitionKeyPath)
        {
            Environment.SetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT", environment);
            Environment.SetEnvironmentVariable("FilesToBeIgnored", filesToBeIgnored);
            Environment.SetEnvironmentVariable("GoogleAnalyticsCode", gaCode);
            Environment.SetEnvironmentVariable("ShortenUrl__Hostname", hostname);
            Environment.SetEnvironmentVariable("ShortenUrl__Length", length.ToString());
            Environment.SetEnvironmentVariable("CosmosDb__ConnectionString", connectionString);
            Environment.SetEnvironmentVariable("CosmosDb__DatabaseName", databaseName);
            Environment.SetEnvironmentVariable("CosmosDb__ContainerName", containerName);
            Environment.SetEnvironmentVariable("CosmosDb__PartitionKeyPath", partitionKeyPath);

            var settings = new AppSettings();

            settings.IsProduction.Should().BeFalse();
            settings.FilesToBeIgnired.Should().BeEquivalentTo(filesToBeIgnored.Split(',', StringSplitOptions.RemoveEmptyEntries));
            settings.GoogleAnalyticsCode.Should().Be(gaCode);

            settings.ShortenUrl.Should().NotBeNull();
            settings.ShortenUrl.Hostname.Should().Be(hostname);
            settings.ShortenUrl.Length.Should().Be(length);

            settings.CosmosDb.Should().NotBeNull();
            settings.CosmosDb.ConnectionString.Should().Be(connectionString);
            settings.CosmosDb.DatabaseName.Should().Be(databaseName);
            settings.CosmosDb.ContainerName.Should().Be(containerName);
            settings.CosmosDb.PartitionKeyPath.Should().Be(partitionKeyPath);
        }
    }
}

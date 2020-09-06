using System;
using System.Threading;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Models.Configurations;
using DevRelKr.UrlShortener.Tests.Utilities;

using FluentAssertions;

using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace DevRelKr.UrlShortener.Repositories.Tests
{
    [TestClass]
    public class CosmosDbContainerHelperTests
    {
        private AppSettingsMocker _mocker;

        [TestInitialize]
        public void Init()
        {
            Environment.SetEnvironmentVariable("FilesToBeIgnored", "favicon.ico");
            Environment.SetEnvironmentVariable("GoogleAnalyticsCode", "UA-123456-7");

            this._mocker = new AppSettingsMocker();
        }

        [TestMethod]
        public void Given_Null_When_Initiated_Then_It_Should_Throw_Exception()
        {
            var settings = this._mocker.CreateAppSettingsInstance();

            Action action = () => new CosmosDbContainerHelper(null, null);

            action.Should().Throw<ArgumentNullException>();

            action = () => new CosmosDbContainerHelper(settings.Object, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow("database", "container", "partition")]
        public async Task Given_Parameters_When_GetContainerAsync_Invoked_Then_It_Should_Return_Result(string databaseId, string containerId, string partitionKeyPath)
        {
            var cosmosDb = new Mock<CosmosDbSettings>();
            cosmosDb.SetupGet(p => p.DatabaseName).Returns(databaseId);
            cosmosDb.SetupGet(p => p.ContainerName).Returns(containerId);
            cosmosDb.SetupGet(p => p.PartitionKeyPath).Returns(partitionKeyPath);

            var settings = this._mocker.CreateAppSettingsInstance();
            settings.SetupGet(p => p.CosmosDb).Returns(cosmosDb.Object);

            var container = new Mock<Container>();
            container.SetupGet(p => p.Id).Returns(containerId);

            var containerr = new Mock<ContainerResponse>();
            containerr.SetupGet(p => p.Container).Returns(container.Object);

            var db = new Mock<Database>();
            db.Setup(p => p.CreateContainerIfNotExistsAsync(It.IsAny<ContainerProperties>(), It.IsAny<int?>(), It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(containerr.Object);

            var dbr = new Mock<DatabaseResponse>();
            dbr.SetupGet(p => p.Database).Returns(db.Object);

            var client = new Mock<CosmosClient>();
            client.Setup(p => p.CreateDatabaseIfNotExistsAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<RequestOptions>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(dbr.Object);

            var helper = new CosmosDbContainerHelper(settings.Object, client.Object);

            var result = await helper.GetContainerAsync().ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Id.Should().Be(containerId);
        }
    }
}

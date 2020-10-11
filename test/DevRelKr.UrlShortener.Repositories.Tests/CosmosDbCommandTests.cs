using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Models.DataStores;
using DevRelKr.UrlShortener.Repositories;
using DevRelKr.UrlShortener.Tests.Fakes;

using FluentAssertions;

using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace DevRelKr.UrlShortener.Services.Tests
{
    [TestClass]
    public class CosmosDbCommandTests
    {
        [TestMethod]
        public void Given_Null_When_Initiated_Then_It_Should_Throw_Exception()
        {
            Action action = () => new CosmosDbCommand(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Given_Null_When_UpsertUrlItemEntityAsync_Invoked_Then_It_Should_Throw_Exception()
        {
            var helper = new Mock<ICosmosDbContainerHelper>();

            var command = new CosmosDbCommand(helper.Object);

            Func<Task> func = async () => await command.UpsertItemEntityAsync<FakeItemEntity>(null).ConfigureAwait(false);

            func.Should().Throw<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow("helloworld", "owner", HttpStatusCode.OK)]
        public async Task Given_Values_When_UpsertUrlItemEntityAsync_Invoked_Then_It_Should_Return_Result(string shortUrl, string owner, HttpStatusCode statusCode)
        {
            var record = new UrlItemEntity() { ShortUrl = shortUrl, Owner = owner };

            var item = new Mock<ItemResponse<UrlItemEntity>>();
            item.SetupGet(p => p.Resource).Returns(record);
            item.SetupGet(p => p.StatusCode).Returns(statusCode);

            var container = new Mock<Container>();
            container.Setup(p => p.UpsertItemAsync<UrlItemEntity>(It.IsAny<UrlItemEntity>(), It.IsAny<PartitionKey?>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(item.Object);

            var helper = new Mock<ICosmosDbContainerHelper>();
            helper.Setup(p => p.GetContainerAsync()).ReturnsAsync(container.Object);

            var command = new CosmosDbCommand(helper.Object);

            var result = await command.UpsertItemEntityAsync<UrlItemEntity>(record).ConfigureAwait(false);

            result.Should().Be((int) statusCode);
        }

        [DataTestMethod]
        [DataRow("helloworld", HttpStatusCode.OK)]
        public async Task Given_Values_When_UpsertVisitItemEntityAsync_Invoked_Then_It_Should_Return_Result(string shortUrl, HttpStatusCode statusCode)
        {
            var record = new VisitItemEntity() { ShortUrl = shortUrl };

            var item = new Mock<ItemResponse<VisitItemEntity>>();
            item.SetupGet(p => p.Resource).Returns(record);
            item.SetupGet(p => p.StatusCode).Returns(statusCode);

            var container = new Mock<Container>();
            container.Setup(p => p.UpsertItemAsync<VisitItemEntity>(It.IsAny<VisitItemEntity>(), It.IsAny<PartitionKey?>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(item.Object);

            var helper = new Mock<ICosmosDbContainerHelper>();
            helper.Setup(p => p.GetContainerAsync()).ReturnsAsync(container.Object);

            var command = new CosmosDbCommand(helper.Object);

            var result = await command.UpsertItemEntityAsync<VisitItemEntity>(record).ConfigureAwait(false);

            result.Should().Be((int) statusCode);
        }
    }
}

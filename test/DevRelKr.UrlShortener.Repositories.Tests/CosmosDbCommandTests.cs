using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Models.DataStores;
using DevRelKr.UrlShortener.Repositories;

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

            Func<Task> func = async () => await command.UpsertUrlItemEntityAsync(null).ConfigureAwait(false);

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

            var result = await command.UpsertUrlItemEntityAsync(record).ConfigureAwait(false);

            result.Should().Be((int) statusCode);
        }
    }
}

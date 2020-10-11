using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CosmosDbQueryTests
    {
        [TestMethod]
        public void Given_Null_When_Initiated_Then_It_Should_Throw_Exception()
        {
            Action action = () => new CosmosDbQuery(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Given_Null_When_GetUrlItemEntityAsync_Invoked_Then_It_Should_Throw_Exception()
        {
            var helper = new Mock<ICosmosDbContainerHelper>();

            var query = new CosmosDbQuery(helper.Object);

            Func<Task> func = async () => await query.GetUrlItemEntityAsync(null).ConfigureAwait(false);

            func.Should().Throw<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow("helloworld")]
        public async Task Given_Values_When_GetUrlItemEntityAsync_Invoked_Then_It_Should_Return_Null(string shortUrl)
        {
            var iterator = new Mock<FeedIterator<UrlItemEntity>>();
            iterator.SetupGet(p => p.HasMoreResults).Returns(false);

            var container = new Mock<Container>();
            container.Setup(p => p.GetItemQueryIterator<UrlItemEntity>(It.IsAny<QueryDefinition>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>()))
                     .Returns(iterator.Object);

            var helper = new Mock<ICosmosDbContainerHelper>();
            helper.Setup(p => p.GetContainerAsync()).ReturnsAsync(container.Object);

            var query = new CosmosDbQuery(helper.Object);

            var result = await query.GetUrlItemEntityAsync(shortUrl).ConfigureAwait(false);

            result.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("helloworld")]
        public async Task Given_Values_When_GetUrlItemEntityAsync_Invoked_Then_It_Should_Return_Result(string shortUrl)
        {
            var record = new UrlItemEntity() { ShortUrl = shortUrl };
            var records = new List<UrlItemEntity>() { record };

            var feed = new Mock<FeedResponse<UrlItemEntity>>();
            feed.SetupGet(p => p.Resource).Returns(records);
            feed.Setup(p => p.GetEnumerator()).Returns(records.GetEnumerator());

            var iterator = new Mock<FeedIterator<UrlItemEntity>>();
            iterator.SetupSequence(p => p.HasMoreResults)
                    .Returns(true)
                    .Returns(false);
            iterator.Setup(p => p.ReadNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(feed.Object);

            var container = new Mock<Container>();
            container.Setup(p => p.GetItemQueryIterator<UrlItemEntity>(It.IsAny<QueryDefinition>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>()))
                     .Returns(iterator.Object);

            var helper = new Mock<ICosmosDbContainerHelper>();
            helper.Setup(p => p.GetContainerAsync()).ReturnsAsync(container.Object);

            var query = new CosmosDbQuery(helper.Object);

            var result = await query.GetUrlItemEntityAsync(shortUrl).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.ShortUrl.Should().Be(shortUrl);
        }

        [TestMethod]
        public void Given_Null_When_GetUrlItemEntityCollectionAsync_Invoked_Then_It_Should_Throw_Exception()
        {
            var helper = new Mock<ICosmosDbContainerHelper>();

            var query = new CosmosDbQuery(helper.Object);

            Func<Task> func = async () => await query.GetUrlItemEntityCollectionAsync(null).ConfigureAwait(false);

            func.Should().Throw<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow("owner")]
        public async Task Given_Values_When_GetUrlItemEntityCollectionAsync_Invoked_Then_It_Should_Return_Null(string owner)
        {
            var iterator = new Mock<FeedIterator<UrlItemEntity>>();
            iterator.SetupGet(p => p.HasMoreResults).Returns(false);

            var container = new Mock<Container>();
            container.Setup(p => p.GetItemQueryIterator<UrlItemEntity>(It.IsAny<QueryDefinition>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>()))
                     .Returns(iterator.Object);

            var helper = new Mock<ICosmosDbContainerHelper>();
            helper.Setup(p => p.GetContainerAsync()).ReturnsAsync(container.Object);

            var query = new CosmosDbQuery(helper.Object);

            var result = await query.GetUrlItemEntityCollectionAsync(owner).ConfigureAwait(false);

            result.Items.Should().HaveCount(0);
        }

        [DataTestMethod]
        [DataRow("owner")]
        public async Task Given_Values_When_GetUrlItemEntityCollectionAsync_Invoked_Then_It_Should_Return_Result(string owner)
        {
            var records = new List<UrlItemEntity>()
            {
                new UrlItemEntity() { ShortUrl = "helloworld", Owner = owner, DateGenerated = DateTimeOffset.UtcNow.AddHours(-1) },
                new UrlItemEntity() { ShortUrl = "loremipsum", Owner = owner, DateGenerated = DateTimeOffset.UtcNow.AddHours(-2) }
            };

            var feed = new Mock<FeedResponse<UrlItemEntity>>();
            feed.SetupGet(p => p.Resource).Returns(records);
            feed.Setup(p => p.GetEnumerator()).Returns(records.GetEnumerator());

            var iterator = new Mock<FeedIterator<UrlItemEntity>>();
            iterator.SetupSequence(p => p.HasMoreResults)
                    .Returns(true)
                    .Returns(false);
            iterator.Setup(p => p.ReadNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(feed.Object);

            var container = new Mock<Container>();
            container.Setup(p => p.GetItemQueryIterator<UrlItemEntity>(It.IsAny<QueryDefinition>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>()))
                     .Returns(iterator.Object);

            var helper = new Mock<ICosmosDbContainerHelper>();
            helper.Setup(p => p.GetContainerAsync()).ReturnsAsync(container.Object);

            var query = new CosmosDbQuery(helper.Object);

            var result = await query.GetUrlItemEntityCollectionAsync(owner).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Items.Should().HaveCount(records.Count);
            result.Items.All(p => p.Owner == owner).Should().BeTrue();
        }

        [TestMethod]
        public void Given_Null_When_GetVisitItemEntityCollectionAsync_Invoked_Then_It_Should_Throw_Exception()
        {
            var helper = new Mock<ICosmosDbContainerHelper>();

            var query = new CosmosDbQuery(helper.Object);

            Func<Task> func = async () => await query.GetVisitItemEntityCollectionAsync(null).ConfigureAwait(false);

            func.Should().Throw<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow("helloworld")]
        public async Task Given_Values_When_GetVisitItemEntityCollectionAsync_Invoked_Then_It_Should_Return_Null(string shortUrl)
        {
            var iterator = new Mock<FeedIterator<VisitItemEntity>>();
            iterator.SetupGet(p => p.HasMoreResults).Returns(false);

            var container = new Mock<Container>();
            container.Setup(p => p.GetItemQueryIterator<VisitItemEntity>(It.IsAny<QueryDefinition>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>()))
                     .Returns(iterator.Object);

            var helper = new Mock<ICosmosDbContainerHelper>();
            helper.Setup(p => p.GetContainerAsync()).ReturnsAsync(container.Object);

            var query = new CosmosDbQuery(helper.Object);

            var result = await query.GetVisitItemEntityCollectionAsync(shortUrl).ConfigureAwait(false);

            result.Items.Should().HaveCount(0);
        }

        [DataTestMethod]
        [DataRow("helloworld")]
        public async Task Given_Values_When_GetVisitItemEntityCollectionAsync_Invoked_Then_It_Should_Return_Result(string shortUrl)
        {
            var records = new List<VisitItemEntity>()
            {
                new VisitItemEntity() { ShortUrl = shortUrl, DateGenerated = DateTimeOffset.UtcNow.AddHours(-1) },
                new VisitItemEntity() { ShortUrl = shortUrl, DateGenerated = DateTimeOffset.UtcNow.AddHours(-2) }
            };

            var feed = new Mock<FeedResponse<VisitItemEntity>>();
            feed.SetupGet(p => p.Resource).Returns(records);
            feed.Setup(p => p.GetEnumerator()).Returns(records.GetEnumerator());

            var iterator = new Mock<FeedIterator<VisitItemEntity>>();
            iterator.SetupSequence(p => p.HasMoreResults)
                    .Returns(true)
                    .Returns(false);
            iterator.Setup(p => p.ReadNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(feed.Object);

            var container = new Mock<Container>();
            container.Setup(p => p.GetItemQueryIterator<VisitItemEntity>(It.IsAny<QueryDefinition>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>()))
                     .Returns(iterator.Object);

            var helper = new Mock<ICosmosDbContainerHelper>();
            helper.Setup(p => p.GetContainerAsync()).ReturnsAsync(container.Object);

            var query = new CosmosDbQuery(helper.Object);

            var result = await query.GetVisitItemEntityCollectionAsync(shortUrl).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Items.Should().HaveCount(records.Count);
            result.Items.All(p => p.ShortUrl == shortUrl).Should().BeTrue();
        }
    }
}

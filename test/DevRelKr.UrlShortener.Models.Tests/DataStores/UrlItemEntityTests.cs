using System;

using DevRelKr.UrlShortener.Models.DataStores;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

namespace DevRelKr.UrlShortener.Models.Tests.DataStores
{
    [TestClass]
    public class UrlItemEntityTests
    {
        [TestMethod]
        public void Given_Type_When_Instantiated_Then_It_Should_Return_Result()
        {
            var entity = new UrlItemEntity();

            entity.CoOwners.Should().NotBeNull()
                           .And.HaveCount(0);
        }

        [TestMethod]
        public void Given_InvalidDateUpdated_When_Assigned_Then_It_Should_Throw_Exception()
        {
            var entity = new UrlItemEntity();

            Action action = () => entity.DateUpdated = DateTimeOffset.MinValue;

            action.Should().Throw<InvalidOperationException>();
        }

        [DataTestMethod]
        [DataRow(null, null, null)]
        [DataRow("https://localhost/loremipsum", null, null)]
        [DataRow("https://localhost/loremipsum", "helloworld", null)]
        public void Given_Null_When_Serialised_Then_It_Should_Throw_Exception(string original, string shortUrl, string owner)
        {
            var entity = new UrlItemEntity()
            {
                OriginalUrl = string.IsNullOrWhiteSpace(original) ? null : new Uri(original),
                ShortUrl = shortUrl,
                Owner = owner,
                DateGenerated = DateTimeOffset.UtcNow,
                DateUpdated = DateTimeOffset.UtcNow
            };

            Action action = () => JsonConvert.SerializeObject(entity);

            action.Should().Throw<JsonSerializationException>();
        }

        [DataTestMethod]
        [DataRow("https://localhost/loremipsum", "helloworld", "owner")]
        public void Given_Values_When_Serialised_Then_It_Should_Return_Result(string original, string shortUrl, string owner)
        {
            var entityId = Guid.NewGuid();
            var collection = PartitionType.Url;
            var uri = new Uri(original);
            var now = DateTimeOffset.UtcNow;

            var entity = new UrlItemEntity()
            {
                EntityId = entityId,
                OriginalUrl = new Uri(original),
                ShortUrl = shortUrl,
                Owner = owner,
                DateGenerated = now,
                DateUpdated = now
            };

            var dateGenerated = JsonConvert.SerializeObject(now);
            var dateUpdated = JsonConvert.SerializeObject(now);

            var serialised = JsonConvert.SerializeObject(entity);

            serialised.Should().Contain($"\"id\":\"{entityId.ToString()}\"");
            serialised.Should().Contain($"\"collection\":\"{collection.ToString()}\"");
            serialised.Should().Contain($"\"originalUrl\":\"{uri.ToString()}\"");
            serialised.Should().Contain($"\"shortUrl\":\"{shortUrl}\"");
            serialised.Should().Contain($"\"owner\":\"{owner}\"");
            serialised.Should().Contain($"\"dateGenerated\":{dateGenerated}");
            serialised.Should().Contain($"\"dateUpdated\":{dateUpdated}");
        }

        [DataTestMethod]
        [DataRow(PartitionType.Url)]
        public void Given_PartitionType_Then_It_Should_Return_Result(PartitionType collection)
        {
            var entity = new UrlItemEntity();

            entity.PartitionKey.Should().Be(collection.ToString());
            entity.PartitionKeyPath.Should().Be("/collection");
        }
    }
}

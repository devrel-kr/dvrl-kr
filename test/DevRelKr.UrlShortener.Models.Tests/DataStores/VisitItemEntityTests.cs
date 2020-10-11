using System;
using System.Collections.Generic;
using DevRelKr.UrlShortener.Models.DataStores;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

namespace DevRelKr.UrlShortener.Models.Tests.DataStores
{
    [TestClass]
    public class VisitItemEntityTests
    {
        [TestMethod]
        public void Given_NullShortUrl_When_Serialised_Then_It_Should_Throw_Exception()
        {
            var entity = new VisitItemEntity()
            {
                ShortUrl = null,
                DateGenerated = DateTimeOffset.UtcNow,
            };

            Action action = () => JsonConvert.SerializeObject(entity);

            action.Should().Throw<JsonSerializationException>();
        }

        [DataTestMethod]
        [DataRow("helloworld")]
        public void Given_Values_When_Serialised_Then_It_Should_Return_Result(string shortUrl)
        {
            var entityId = Guid.NewGuid();
            var collection = PartitionType.Visit;
            var now = DateTimeOffset.UtcNow;

            var entity = new VisitItemEntity()
            {
                EntityId = entityId,
                ShortUrl = shortUrl,
                DateGenerated = now,
            };

            var dateGenerated = JsonConvert.SerializeObject(now);

            var serialised = JsonConvert.SerializeObject(entity);

            serialised.Should().Contain($"\"id\":\"{entityId.ToString()}\"");
            serialised.Should().Contain($"\"collection\":\"{collection.ToString()}\"");
            serialised.Should().Contain($"\"shortUrl\":\"{shortUrl}\"");
            serialised.Should().Contain($"\"dateGenerated\":{dateGenerated}");
            serialised.Should().Contain($"\"requestHeaders\":{{}}");
        }

        [DataTestMethod]
        [DataRow(PartitionType.Visit)]
        public void Given_PartitionType_Then_It_Should_Return_Result(PartitionType collection)
        {
            var entity = new VisitItemEntity() { Collection = collection };

            entity.PartitionKey.Should().Be(collection.ToString());
            entity.PartitionKeyPath.Should().Be("/collection");
        }
    }
}

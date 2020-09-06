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
            };

            Action action = () => JsonConvert.SerializeObject(entity);

            action.Should().Throw<JsonSerializationException>();
        }

        [DataTestMethod]
        [DataRow("https://localhost/loremipsum", "helloworld", "owner", false, false)]
        [DataRow("https://localhost/loremipsum", "helloworld", "owner", true, false)]
        public void Given_Values_When_Serialised_Then_It_Should_Return_Result(string original, string shortUrl, string owner, bool useDateGenerated, bool useDateUpdated)
        {
            var entityId = Guid.NewGuid();

            var entity = new UrlItemEntity()
            {
                EntityId = entityId,
                OriginalUrl = new Uri(original),
                ShortUrl = shortUrl,
                Owner = owner,
            };

            var now = DateTimeOffset.UtcNow;
            if (useDateGenerated)
            {
                entity.DateGenerated = now;
            }
            if (useDateUpdated)
            {
                entity.DateUpdated = now;
            }

            var dateGenerated = JsonConvert.SerializeObject(entity.DateGenerated);
            var dateUpdated = JsonConvert.SerializeObject(entity.DateUpdated);

            var serialised = JsonConvert.SerializeObject(entity);

            serialised.Should().Contain($"\"id\":\"{entityId.ToString()}\"");
            serialised.Should().Contain($"\"dateGenerated\":{dateGenerated}");
            serialised.Should().Contain($"\"dateUpdated\":{dateUpdated}");
        }

        [DataTestMethod]
        [DataRow("natasha")]
        public void Given_Owner_Then_It_Should_Return_Result(string owner)
        {
            var entity = new UrlItemEntity() { Owner = owner };

            entity.PartitionKey.Should().Be(owner);
            entity.PartitionKeyPath.Should().Be("/owner");
        }
    }
}

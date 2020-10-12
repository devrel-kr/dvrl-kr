using System;
using System.Reflection;

using DevRelKr.UrlShortener.Tests.Fakes;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

namespace DevRelKr.UrlShortener.Models.Tests.DataStores
{
    [TestClass]
    public class ItemEntityTests
    {
        [TestMethod]
        public void Given_InvalidEntityId_When_Assigned_Then_It_Should_Throw_Exception()
        {
            var entity = new FakeItemEntity();

            Action action = () => entity.EntityId = Guid.Empty;

            action.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void Given_InvalidCollection_When_Assigned_Then_It_Should_Throw_Exception()
        {
            var entity = new FakeItemEntity();

            Action action = () => entity.Collection = PartitionType.None;

            action.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void Given_InvalidDateGenerated_When_Assigned_Then_It_Should_Throw_Exception()
        {
            var entity = new FakeItemEntity();

            Action action = () => entity.DateGenerated = DateTimeOffset.MinValue;

            action.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void Given_EntityId_When_Decorator_Captured_Then_It_Should_Have_PropertyOrderOf_MinValue()
        {
            var entity = new FakeItemEntity();
            var pi = typeof(FakeItemEntity).GetProperty("EntityId", BindingFlags.Public | BindingFlags.Instance);
            var attribute = pi.GetCustomAttribute<JsonPropertyAttribute>(inherit: false);

            attribute.Order.Should().Be(int.MinValue);
        }

        [TestMethod]
        public void Given_Collection_When_Decorator_Captured_Then_It_Should_Have_PropertyOrderOf_MinValue()
        {
            var entity = new FakeItemEntity();
            var pi = typeof(FakeItemEntity).GetProperty("Collection", BindingFlags.Public | BindingFlags.Instance);
            var attribute = pi.GetCustomAttribute<JsonPropertyAttribute>(inherit: false);

            attribute.Order.Should().Be(int.MinValue);
        }

        [TestMethod]
        public void Given_DateGenerated_When_Decorator_Captured_Then_It_Should_Have_PropertyOrderOf_MaxValue()
        {
            var entity = new FakeItemEntity();
            var pi = typeof(FakeItemEntity).GetProperty("DateGenerated", BindingFlags.Public | BindingFlags.Instance);
            var attribute = pi.GetCustomAttribute<JsonPropertyAttribute>(inherit: false);

            attribute.Order.Should().Be(int.MaxValue);
        }

        [DataTestMethod]
        [DataRow(PartitionType.Url)]
        [DataRow(PartitionType.Visit)]
        public void Given_Values_When_Serialised_Then_It_Should_Return_Result(PartitionType collection)
        {
            var entityId = Guid.NewGuid();
            var now = DateTimeOffset.UtcNow;

            var entity = new FakeItemEntity()
            {
                EntityId = entityId,
                Collection = collection,
                DateGenerated = now
            };

            var dateGenerated = JsonConvert.SerializeObject(now);

            var serialised = JsonConvert.SerializeObject(entity);

            serialised.Should().Contain($"\"id\":\"{entityId.ToString()}\"");
            serialised.Should().Contain($"\"collection\":\"{collection.ToString()}\"");
            serialised.Should().Contain($"\"dateGenerated\":{dateGenerated}");
        }

        [DataTestMethod]
        [DataRow(PartitionType.Url)]
        [DataRow(PartitionType.Visit)]
        public void Given_Collection_Then_It_Should_Return_Result(PartitionType collection)
        {
            var entity = new FakeItemEntity() { Collection = collection };

            entity.PartitionKey.Should().Be(collection.ToString());
            entity.PartitionKeyPath.Should().Be("/collection");
        }
    }
}

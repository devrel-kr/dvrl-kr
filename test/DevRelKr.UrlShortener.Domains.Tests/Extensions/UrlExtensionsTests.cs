using System;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Domains.Extensions;
using DevRelKr.UrlShortener.Models.Responses;
using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace DevRelKr.UrlShortener.Domains.Tests.Extensions
{
    [TestClass]
    public class UrlExtensionsTests
    {
        [TestMethod]
        public async Task Given_Instance_When_ValidateAsync_Invoked_Then_It_Should_Return_Result()
        {
            var instance = new Mock<IUrl>();
            instance.Setup(p => p.ValidateAsync()).ReturnsAsync(instance.Object);

            var value = Task.FromResult(instance.Object);

            var result = await UrlExtensions.ValidateAsync(value).ConfigureAwait(false);

            result.Should().Be(instance.Object);
        }

        [TestMethod]
        public async Task Given_Instance_When_ShortenAsync_Invoked_Then_It_Should_Return_Result()
        {
            var instance = new Mock<IUrl>();
            instance.Setup(p => p.ShortenAsync()).ReturnsAsync(instance.Object);

            var value = Task.FromResult(instance.Object);

            var result = await UrlExtensions.ShortenAsync(value).ConfigureAwait(false);

            result.Should().Be(instance.Object);
        }

        [TestMethod]
        public async Task Given_Instance_When_ExpandAsync_Invoked_Then_It_Should_Return_Result()
        {
            var instance = new Mock<IUrl>();
            instance.Setup(p => p.ExpandAsync()).ReturnsAsync(instance.Object);

            var value = Task.FromResult(instance.Object);

            var result = await UrlExtensions.ExpandAsync(value).ConfigureAwait(false);

            result.Should().Be(instance.Object);
        }

        [TestMethod]
        public async Task Given_Instance_When_AddHitCountAsync_With_ShortenerResponse_Invoked_Then_It_Should_Return_Result()
        {
            var instance = new Mock<IUrl>();
            instance.Setup(p => p.AddHitCountAsync<ShortenerResponse>()).ReturnsAsync(instance.Object);

            var value = Task.FromResult(instance.Object);

            var result = await UrlExtensions.AddHitCountAsync<ShortenerResponse>(value).ConfigureAwait(false);

            result.Should().Be(instance.Object);
        }

        [TestMethod]
        public async Task Given_Instance_When_AddHitCountAsync_With_ExpanderResponse_Invoked_Then_It_Should_Return_Result()
        {
            var instance = new Mock<IUrl>();
            instance.Setup(p => p.AddHitCountAsync<ExpanderResponse>()).ReturnsAsync(instance.Object);

            var value = Task.FromResult(instance.Object);

            var result = await UrlExtensions.AddHitCountAsync<ExpanderResponse>(value).ConfigureAwait(false);

            result.Should().Be(instance.Object);
        }

        [TestMethod]
        public async Task Given_Instance_When_CreateRecordAsync_Invoked_Then_It_Should_Return_Result()
        {
            var instance = new Mock<IUrl>();
            instance.Setup(p => p.CreateRecordAsync(It.IsAny<DateTimeOffset>(), It.IsAny<Guid>())).ReturnsAsync(instance.Object);

            var value = Task.FromResult(instance.Object);

            var now = DateTimeOffset.UtcNow;
            var entityId = Guid.NewGuid();

            var result = await UrlExtensions.CreateRecordAsync(value, now, entityId).ConfigureAwait(false);

            result.Should().Be(instance.Object);
        }

        [TestMethod]
        public async Task Given_Instance_With_Null_EntityId_When_UpdateRecordAsync_Invoked_Then_It_Should_Return_Result()
        {
            var instance = new Mock<IUrl>();
            instance.Setup(p => p.UpdateRecordAsync<UrlResponse>(It.IsAny<DateTimeOffset>(), It.IsAny<Guid?>())).ReturnsAsync(instance.Object);

            var value = Task.FromResult(instance.Object);

            var now = DateTimeOffset.UtcNow;
            var entityId = (Guid?)null;

            var result = await UrlExtensions.UpdateRecordAsync<UrlResponse>(value, now, entityId).ConfigureAwait(false);

            result.Should().Be(instance.Object);
        }

        [TestMethod]
        public async Task Given_Instance_When_UpdateRecordAsync_Invoked_Then_It_Should_Return_Result()
        {
            var instance = new Mock<IUrl>();
            instance.Setup(p => p.UpdateRecordAsync<UrlResponse>(It.IsAny<DateTimeOffset>(), It.IsAny<Guid?>())).ReturnsAsync(instance.Object);

            var value = Task.FromResult(instance.Object);

            var now = DateTimeOffset.UtcNow;
            var entityId = Guid.NewGuid();

            var result = await UrlExtensions.UpdateRecordAsync<UrlResponse>(value, now, entityId).ConfigureAwait(false);

            result.Should().Be(instance.Object);
        }
    }
}

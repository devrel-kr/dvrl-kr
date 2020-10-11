using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Models.Configurations;
using DevRelKr.UrlShortener.Models.DataStores;
using DevRelKr.UrlShortener.Models.Requests;
using DevRelKr.UrlShortener.Models.Responses;
using DevRelKr.UrlShortener.Repositories;
using DevRelKr.UrlShortener.Tests.Utilities;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace DevRelKr.UrlShortener.Services.Tests
{
    [TestClass]
    public class ShortenerServiceTests
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
            var query = new Mock<IQuery>();

            Action action = () => new ShortenerService(null, null, null);

            action.Should().Throw<ArgumentNullException>();

            action = () => new ShortenerService(settings.Object, null, null);

            action.Should().Throw<ArgumentNullException>();

            action = () => new ShortenerService(settings.Object, query.Object, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Given_Null_When_ShortenAsync_Invoked_Then_It_Should_Throw_Exception()
        {
            var settings = this._mocker.CreateAppSettingsInstance();
            var query = new Mock<IQuery>();
            var command = new Mock<ICommand>();

            var service = new ShortenerService(settings.Object, query.Object, command.Object);

            Func<Task> func = async () => await service.ShortenAsync(null).ConfigureAwait(false);

            func.Should().Throw<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow("dvrl.kr", 10, "https://localhost/loremipsum", "home")]
        [DataRow("dvrl.kr", 10, "https://localhost/loremipsum", "hello/world")]
        public async Task Given_FriendlyUrl_When_ShortenAsync_Invoked_Then_It_Should_Return_Result(string hostname, int length, string original, string friendly)
        {
            var shortenUrl = new Mock<ShortenUrlSettings>();
            shortenUrl.SetupGet(p => p.Hostname).Returns(hostname);
            shortenUrl.SetupGet(p => p.Length).Returns(length);

            var settings = this._mocker.CreateAppSettingsInstance();
            settings.SetupGet(p => p.ShortenUrl).Returns(shortenUrl.Object);

            var query = new Mock<IQuery>();
            var command = new Mock<ICommand>();

            var service = new ShortenerService(settings.Object, query.Object, command.Object);

            var payload = new ShortenerRequest()
            {
                Original = new Uri(original),
                Friendly = friendly
            };

            var result = await service.ShortenAsync(payload).ConfigureAwait(false);

            result.Original.ToString().TrimEnd('/').Should().Be(original.TrimEnd('/'));
            result.Shortened.ToString().TrimEnd('/').Should().Be($"https://{hostname}/{friendly.TrimEnd('/')}");
            result.ShortUrl.Should().Be(friendly);
        }

        [DataTestMethod]
        [DataRow("dvrl.kr", 10, "https://localhost/loremipsum")]
        public async Task Given_NoFriendlyUrl_When_ShortenAsync_Invoked_Then_It_Should_Return_Result(string hostname, int length, string original)
        {
            var shortenUrl = new Mock<ShortenUrlSettings>();
            shortenUrl.SetupGet(p => p.Hostname).Returns(hostname);
            shortenUrl.SetupGet(p => p.Length).Returns(length);

            var settings = this._mocker.CreateAppSettingsInstance();
            settings.SetupGet(p => p.ShortenUrl).Returns(shortenUrl.Object);

            var query = new Mock<IQuery>();
            var command = new Mock<ICommand>();

            var service = new ShortenerService(settings.Object, query.Object, command.Object);

            var payload = new ShortenerRequest()
            {
                Original = new Uri(original)
            };

            var shortened = string.Join(string.Empty, Enumerable.Range(1, length).Select(_ => "a"));
            var result = await service.ShortenAsync(payload).ConfigureAwait(false);

            result.Original.ToString().TrimEnd('/').Should().Be(original.TrimEnd('/'));
            result.Shortened.ToString().TrimEnd('/').Length.Should().Be($"https://{hostname}/{shortened}".Length);
            result.ShortUrl.Length.Should().Be(shortened.Length);
        }

        [TestMethod]
        public void Given_Null_When_ExistsAsync_Invoked_Then_It_Should_Throw_Exception()
        {
            var settings = this._mocker.CreateAppSettingsInstance();
            var query = new Mock<IQuery>();
            var command = new Mock<ICommand>();

            var service = new ShortenerService(settings.Object, query.Object, command.Object);

            Func<Task> func = async () => await service.ExistsAsync(null).ConfigureAwait(false);

            func.Should().Throw<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow("helloworld")]
        public async Task Given_ShortUrl_When_ExistsAsync_Invoked_Then_It_Should_Return_False(string shortUrl)
        {
            var settings = this._mocker.CreateAppSettingsInstance();

            var item = default(UrlItemEntity);

            var query = new Mock<IQuery>();
            query.Setup(p => p.GetUrlItemEntityAsync(It.IsAny<string>())).ReturnsAsync(item);

            var command = new Mock<ICommand>();

            var service = new ShortenerService(settings.Object, query.Object, command.Object);

            var result = await service.ExistsAsync(shortUrl).ConfigureAwait(false);

            result.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("helloworld")]
        public async Task Given_ShortUrl_When_ExistsAsync_Invoked_Then_It_Should_Return_True(string shortUrl)
        {
            var settings = this._mocker.CreateAppSettingsInstance();

            var item = new UrlItemEntity();

            var query = new Mock<IQuery>();
            query.Setup(p => p.GetUrlItemEntityAsync(It.IsAny<string>())).ReturnsAsync(item);

            var command = new Mock<ICommand>();

            var service = new ShortenerService(settings.Object, query.Object, command.Object);

            var result = await service.ExistsAsync(shortUrl).ConfigureAwait(false);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void Given_Null_When_UpsertAsync_Invoked_Then_It_Should_Throw_Exception()
        {
            var settings = this._mocker.CreateAppSettingsInstance();
            var query = new Mock<IQuery>();
            var command = new Mock<ICommand>();

            var service = new ShortenerService(settings.Object, query.Object, command.Object);

            Func<Task> func = async () => await service.UpsertAsync(null).ConfigureAwait(false);

            func.Should().Throw<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.OK)]
        public async Task Given_ShortUrl_When_UpsertAsync_Invoked_Then_It_Should_Return_Result(HttpStatusCode statusCode)
        {
            var settings = this._mocker.CreateAppSettingsInstance();

            var item = new UrlItemEntity();
            var query = new Mock<IQuery>();

            var command = new Mock<ICommand>();
            command.Setup(p => p.UpsertItemEntityAsync<UrlItemEntity>(It.IsAny<UrlItemEntity>())).ReturnsAsync((int) statusCode);

            var service = new ShortenerService(settings.Object, query.Object, command.Object);

            var payload = new ShortenerResponse()
            {
                EntityId = Guid.NewGuid(),
                DateGenerated = DateTimeOffset.UtcNow,
                DateUpdated = DateTimeOffset.UtcNow
            };

            var result = await service.UpsertAsync(payload).ConfigureAwait(false);

            result.Should().Be((int) statusCode);
        }
    }
}

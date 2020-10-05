using System;
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
    public class ExpanderServiceTests
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

            Action action = () => new ExpanderService(null, null, null);

            action.Should().Throw<ArgumentNullException>();

            action = () => new ExpanderService(settings.Object, null, null);

            action.Should().Throw<ArgumentNullException>();

            action = () => new ExpanderService(settings.Object, query.Object, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Given_Null_When_ExpandAsync_Invoked_Then_It_Should_Throw_Exception()
        {
            var settings = this._mocker.CreateAppSettingsInstance();
            var query = new Mock<IQuery>();
            var command = new Mock<ICommand>();

            var service = new ExpanderService(settings.Object, query.Object, command.Object);

            Func<Task> func = async () => await service.ExpandAsync(null).ConfigureAwait(false);

            func.Should().Throw<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow("dvrl.kr", "helloworld", "https://localhost/loremipsum")]
        public async Task Given_ShortUrl_When_ExpandAsync_Invoked_Then_It_Should_Return_Result(string hostname, string shortUrl, string original)
        {
            var shorten = new Mock<ShortenUrlSettings>();
            shorten.SetupGet(p => p.Hostname).Returns(hostname);

            var settings = this._mocker.CreateAppSettingsInstance();
            settings.SetupGet(p => p.ShortenUrl).Returns(shorten.Object);

            var item = new UrlItemEntity()
            {
                ShortUrl = shortUrl,
                OriginalUrl = new Uri(original)
            };

            var query = new Mock<IQuery>();
            query.Setup(p => p.GetUrlItemEntityAsync(It.IsAny<string>())).ReturnsAsync(item);

            var command = new Mock<ICommand>();

            var service = new ExpanderService(settings.Object, query.Object, command.Object);

            var payload = new ExpanderRequest() { ShortUrl = shortUrl };

            var result = await service.ExpandAsync(payload).ConfigureAwait(false);

            result.ShortUrl.Should().Be(shortUrl);
            result.Original.ToString().TrimEnd('/').Should().Be(original.TrimEnd('/'));
            result.Shortened.ToString().TrimEnd('/').Should().Be($"https://{hostname}/{shortUrl}");
        }

        [TestMethod]
        public void Given_Null_When_UpsertAsync_Invoked_Then_It_Should_Throw_Exception()
        {
            var settings = this._mocker.CreateAppSettingsInstance();
            var query = new Mock<IQuery>();
            var command = new Mock<ICommand>();

            var service = new ExpanderService(settings.Object, query.Object, command.Object);

            Func<Task> func = async () => await service.UpsertAsync<UrlItemEntity>(null).ConfigureAwait(false);

            func.Should().Throw<ArgumentNullException>();

            func = async () => await service.UpsertAsync<VisitItemEntity>(null).ConfigureAwait(false);

            func.Should().Throw<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.OK)]
        public async Task Given_ShortUrl_When_UpsertAsync_With_UrlItemEntity_Invoked_Then_It_Should_Return_Result(HttpStatusCode statusCode)
        {
            var settings = this._mocker.CreateAppSettingsInstance();

            var item = new UrlItemEntity();
            var query = new Mock<IQuery>();

            var command = new Mock<ICommand>();
            command.Setup(p => p.UpsertItemEntityAsync<UrlItemEntity>(It.IsAny<UrlItemEntity>())).ReturnsAsync((int) statusCode);

            var service = new ExpanderService(settings.Object, query.Object, command.Object);

            var payload = new ExpanderResponse()
            {
                EntityId = Guid.NewGuid(),
                DateGenerated = DateTimeOffset.UtcNow,
                DateUpdated = DateTimeOffset.UtcNow
            };

            var result = await service.UpsertAsync<UrlItemEntity>(payload).ConfigureAwait(false);

            result.Should().Be((int) statusCode);
        }

        [DataTestMethod]
        [DataRow(HttpStatusCode.OK)]
        public async Task Given_ShortUrl_When_UpsertAsync_With_VisitItemEntity_Invoked_Then_It_Should_Return_Result(HttpStatusCode statusCode)
        {
            var settings = this._mocker.CreateAppSettingsInstance();

            var item = new VisitItemEntity();
            var query = new Mock<IQuery>();

            var command = new Mock<ICommand>();
            command.Setup(p => p.UpsertItemEntityAsync<VisitItemEntity>(It.IsAny<VisitItemEntity>())).ReturnsAsync((int) statusCode);

            var service = new ExpanderService(settings.Object, query.Object, command.Object);

            var payload = new ExpanderResponse()
            {
                EntityId = Guid.NewGuid(),
                DateGenerated = DateTimeOffset.UtcNow,
                DateUpdated = DateTimeOffset.UtcNow
            };

            var result = await service.UpsertAsync<VisitItemEntity>(payload).ConfigureAwait(false);

            result.Should().Be((int) statusCode);
        }
    }
}

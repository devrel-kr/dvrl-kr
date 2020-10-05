using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Domains;
using DevRelKr.UrlShortener.Domains.Exceptions;
using DevRelKr.UrlShortener.Models.Responses;
using DevRelKr.UrlShortener.Tests.Utilities;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace DevRelKr.UrlShortener.FunctionApp.Tests
{
    [TestClass]
    public class ShortenUrlHttpTriggerTests
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

            Action action = () => new ShortenUrlHttpTrigger(null, null);

            action.Should().Throw<ArgumentNullException>();

            action = () => new ShortenUrlHttpTrigger(settings.Object, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public async Task Given_Request_When_ShortenUrl_Invoked_Then_It_Should_Return_ErrorResponse()
        {
            var settings = this._mocker.CreateAppSettingsInstance();

            var url = new Mock<IUrl>();
            url.Setup(p => p.GetRequestAsync(It.IsAny<HttpRequest>())).Throws<ArgumentNullException>();

            var trigger = new ShortenUrlHttpTrigger(settings.Object, url.Object);

            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.IsHttps).Returns(true);

            var log = new Mock<ILogger>();

            var result = await trigger.ShortenUrl(req.Object, log.Object).ConfigureAwait(false);

            result.Should().BeOfType<ObjectResult>();
            (result as ObjectResult).StatusCode.Value.Should().Be((int)HttpStatusCode.InternalServerError);
            (result as ObjectResult).Value.Should().BeOfType<ExceptionResponse>();
        }

        [TestMethod]
        public async Task Given_Request_When_ShortenUrl_Invoked_Then_It_Should_Return_ConflictResponse()
        {
            var settings = this._mocker.CreateAppSettingsInstance();

            var url = new Mock<IUrl>();
            url.Setup(p => p.GetRequestAsync(It.IsAny<HttpRequest>())).ReturnsAsync(url.Object);
            url.Setup(p => p.ValidateAsync()).Throws<UrlExistsException>();

            var trigger = new ShortenUrlHttpTrigger(settings.Object, url.Object);

            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.IsHttps).Returns(true);

            var log = new Mock<ILogger>();

            var result = await trigger.ShortenUrl(req.Object, log.Object).ConfigureAwait(false);

            result.Should().BeOfType<ConflictObjectResult>();
            (result as ConflictObjectResult).Value.Should().BeOfType<ExceptionResponse>();
        }

        [DataTestMethod]
        [DataRow("https://localhost", null, null, null, "owner", "co,owners")]
        [DataRow("https://localhost", "helloworld", null, null, "owner", "co,owners")]
        [DataRow("https://localhost", "helloworld", "title", null, "owner", "co,owners")]
        [DataRow("https://localhost", "helloworld", "title", "description", "owner", "co,owners")]
        public async Task Given_Request_When_ShortenUrl_Invoked_Then_It_Should_Return_Result(string original, string shortUrl, string title, string description, string owner, string coOwners)
        {
            if (string.IsNullOrWhiteSpace(shortUrl))
            {
                shortUrl = "loremipsum";
            }

            var now = DateTimeOffset.UtcNow;
            var shortened = $"https://dvrl.kr/{shortUrl}";
            var owners = coOwners.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            var response = new ShortenerResponse()
            {
                Original = new Uri(original),
                Shortened = new Uri(shortened),
                ShortUrl = shortUrl,
                Title = title,
                Description = description,
                Owner = owner,
                CoOwners = owners,
                DateGenerated = now,
                DateUpdated = now
            };

            var settings = this._mocker.CreateAppSettingsInstance();

            var url = new Mock<IUrl>();
            url.SetupGet(p => p.ShortenerResponse).Returns(response);
            url.Setup(p => p.GetRequestAsync(It.IsAny<HttpRequest>())).ReturnsAsync(url.Object);
            url.Setup(p => p.ValidateAsync()).ReturnsAsync(url.Object);
            url.Setup(p => p.ShortenAsync()).ReturnsAsync(url.Object);
            url.Setup(p => p.CreateRecordAsync(It.IsAny<DateTimeOffset>(), It.IsAny<Guid>())).ReturnsAsync(url.Object);

            var trigger = new ShortenUrlHttpTrigger(settings.Object, url.Object);

            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.IsHttps).Returns(true);

            var log = new Mock<ILogger>();

            var result = await trigger.ShortenUrl(req.Object, log.Object).ConfigureAwait(false);

            result.Should().BeOfType<OkObjectResult>();
            (result as OkObjectResult).Value.Should().BeOfType<ShortenerResponse>();
            ((result as OkObjectResult).Value as ShortenerResponse).Original.Should().Be(original);
            ((result as OkObjectResult).Value as ShortenerResponse).Shortened.Should().Be(shortened);
            ((result as OkObjectResult).Value as ShortenerResponse).ShortUrl.Should().Be(shortUrl);
            ((result as OkObjectResult).Value as ShortenerResponse).Title.Should().Be(title);
            ((result as OkObjectResult).Value as ShortenerResponse).Description.Should().Be(description);
            ((result as OkObjectResult).Value as ShortenerResponse).Owner.Should().Be(owner);
            ((result as OkObjectResult).Value as ShortenerResponse).CoOwners.Should().HaveCount(owners.Count);
            ((result as OkObjectResult).Value as ShortenerResponse).DateGenerated.Should().Be(now);
            ((result as OkObjectResult).Value as ShortenerResponse).DateUpdated.Should().Be(now);
            ((result as OkObjectResult).Value as ShortenerResponse).HitCount.Should().Be(0);
        }
    }
}

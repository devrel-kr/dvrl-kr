using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Domains;
using DevRelKr.UrlShortener.Models.Responses;
using DevRelKr.UrlShortener.Tests.Utilities;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace DevRelKr.UrlShortener.FunctionApp.Tests
{
    [TestClass]
    public class RedirectUrlHttpTriggerTests
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

            Action action = () => new RedirectUrlHttpTrigger(null, null);

            action.Should().Throw<ArgumentNullException>();

            action = () => new RedirectUrlHttpTrigger(settings.Object, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow("https://localhost", "dvrl.kr", null, null, null, "owner", "co,owners")]
        [DataRow("https://localhost", "dvrl.kr", "helloworld", null, null, "owner", "co,owners")]
        [DataRow("https://localhost", "dvrl.kr", "helloworld", "title", null, "owner", "co,owners")]
        [DataRow("https://localhost", "dvrl.kr", "helloworld", "title", "description", "owner", "co,owners")]
        public async Task Given_Request_When_RedirectUrl_Invoked_Then_It_Should_Return_Result(string original, string hostname, string shortUrl, string title, string description, string owner, string coOwners)
        {
            if (string.IsNullOrWhiteSpace(shortUrl))
            {
                shortUrl = "loremipsum";
            }

            var now = DateTimeOffset.UtcNow;
            var shortened = $"https://{hostname}/{shortUrl}";
            var owners = coOwners.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            var response = new ExpanderResponse()
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
            settings.SetupGet(p => p.FilesToBeIgnired).Returns(new List<string>());

            var url = new Mock<IUrl>();
            url.SetupGet(p => p.ExpanderResponse).Returns(response);
            url.SetupGet(p => p.Original).Returns(response.Original);
            url.Setup(p => p.GetRequestAsync(It.IsAny<HttpRequest>(), It.IsAny<string>())).ReturnsAsync(url.Object);
            url.Setup(p => p.ExpandAsync()).ReturnsAsync(url.Object);
            url.Setup(p => p.AddHitCountAsync<ExpanderResponse>()).ReturnsAsync(url.Object);
            url.Setup(p => p.UpdateRecordAsync<ExpanderResponse>(It.IsAny<DateTimeOffset>(), It.IsAny<Guid?>())).ReturnsAsync(url.Object);

            var trigger = new RedirectUrlHttpTrigger(settings.Object, url.Object);

            var requestId = Guid.NewGuid();
            var items = new Dictionary<object, object>() { { "MS_AzureFunctionsRequestID", requestId.ToString() } };

            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(p => p.Items).Returns(items);

            var host = new HostString(hostname);

            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.HttpContext).Returns(httpContext.Object);
            req.SetupGet(p => p.IsHttps).Returns(true);
            req.SetupGet(p => p.Host).Returns(host);

            var assemblyLocation = Assembly.GetAssembly(this.GetType()).Location;
            var segments = assemblyLocation.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            var funcAppDirectory = $"{Path.DirectorySeparatorChar}{string.Join(Path.DirectorySeparatorChar, segments.Take(segments.Count()-1))}";

            var executionContext = new ExecutionContext() { FunctionAppDirectory = funcAppDirectory };
            var log = new Mock<ILogger>();

            var result = await trigger.RedirectUrl(req.Object, shortUrl, executionContext, log.Object).ConfigureAwait(false);

            result.Should().BeOfType<ContentResult>();
            (result as ContentResult).Content.Should().NotContain("{{REDIRECT_URL}}");
            (result as ContentResult).ContentType.Should().Be("text/html");
            (result as ContentResult).StatusCode.Should().Be(200);
        }
    }
}

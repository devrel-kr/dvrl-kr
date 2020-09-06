using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace DevRelKr.UrlShortener.FunctionApp.Tests
{
    [TestClass]
    public class InboundUrlHttpTriggerTests
    {
        [DataTestMethod]
        [DataRow("dvrl.kr", "helloworld")]
        public async Task Given_ShortUrl_When_BounceUrl_Invoked_Then_It_Should_Return_Result(string hostname, string shortUrl)
        {
            var host = new HostString(hostname);

            var trigger = new InboundUrlHttpTrigger();

            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.IsHttps).Returns(true);
            req.SetupGet(p => p.Host).Returns(host);

            var log = new Mock<ILogger>();

            var result = await trigger.BounceUrl(req.Object, shortUrl, log.Object).ConfigureAwait(false);

            result.Should().BeOfType<RedirectResult>();
            (result as RedirectResult).Url.Should().Be($"https://{hostname}/b/{shortUrl}");
        }
    }
}

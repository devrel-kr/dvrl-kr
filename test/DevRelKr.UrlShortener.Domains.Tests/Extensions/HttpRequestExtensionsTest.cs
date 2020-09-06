using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Domains.Extensions;
using DevRelKr.UrlShortener.Models.Requests;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Newtonsoft.Json;

namespace DevRelKr.UrlShortener.Domains.Tests.Extensions
{
    [TestClass]
    public class HttpRequestExtensionsTest
    {
        [TestMethod]
        public void Given_Null_When_GetShortenerRequestAsync_Invoked_Then_It_Should_Throw_Exception()
        {
            Func<Task> func = async () => await HttpRequestExtensions.GetShortenerRequestAsync(null).ConfigureAwait(false);

            func.Should().Throw<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow("PUT")]
        [DataRow("DELETE")]
        [DataRow("PATCH")]
        [DataRow("HEAD")]
        [DataRow("OPTION")]
        public void Given_InvalidMethod_When_GetShortenerRequestAsync_Invoked_Then_It_Should_Throw_Exception(string method)
        {
            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.Method).Returns(method);

            Func<Task> func = async () => await HttpRequestExtensions.GetShortenerRequestAsync(req.Object).ConfigureAwait(false);

            func.Should().Throw<InvalidOperationException>();
        }

        [DataTestMethod]
        [DataRow("GET", null, null)]
        [DataRow("GET", "https://localhost", null)]
        public void Given_Query_When_GetShortenerRequestAsync_Invoked_Then_It_Should_Throw_Exception(string method, string original, string owner)
        {
            var dict = new Dictionary<string, StringValues>()
            {
                { "original", original },
                { "owner", owner },
            };
            var collection = new QueryCollection(dict);

            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.Method).Returns(method);
            req.SetupGet(p => p.Query).Returns(collection);

            Func<Task> func = async () => await HttpRequestExtensions.GetShortenerRequestAsync(req.Object).ConfigureAwait(false);

            func.Should().Throw<InvalidOperationException>();
        }

        [DataTestMethod]
        [DataRow("GET", "https://localhost", null, null, null, "owner")]
        [DataRow("GET", "https://localhost", "home", null, null, "owner")]
        [DataRow("GET", "https://localhost", "home", "title", null, "owner", "co", "owners")]
        [DataRow("GET", "https://localhost", "home", "title", "description", "owner", "co", "owners")]
        public async Task Given_Query_When_GetShortenerRequestAsync_Invoked_Then_It_Should_Return_Result(string method, string original, string friendly, string title, string description, string owner, params string[] coOwners)
        {
            var dict = new Dictionary<string, StringValues>()
            {
                { "original", original },
                { "friendly", friendly },
                { "title", title },
                { "description", description },
                { "owner", owner },
                { "coowners", coOwners },
            };
            var query = new QueryCollection(dict);

            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.Method).Returns(method);
            req.SetupGet(p => p.Query).Returns(query);

            var result = await HttpRequestExtensions.GetShortenerRequestAsync(req.Object).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Original.ToString().TrimEnd('/').Should().Be(original.TrimEnd('/'));
            result.Friendly.Should().Be(friendly);
        }

        [DataTestMethod]
        [DataRow("POST", "https://localhost", null, "owner")]
        [DataRow("POST", "https://localhost", "home", "owner")]
        public async Task Given_Body_When_GetShortenerRequestAsync_Invoked_Then_It_Should_Return_Result(string method, string original, string friendly, string owner)
        {
            var payload = new ShortenerRequest()
            {
                Original = new Uri(original),
                Friendly = friendly,
                Owner = owner
            };
            var serialised = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));
            var body = new MemoryStream(serialised);

            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.Method).Returns(method);
            req.SetupGet(p => p.Body).Returns(body);

            var result = await HttpRequestExtensions.GetShortenerRequestAsync(req.Object).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Original.ToString().TrimEnd('/').Should().Be(original.TrimEnd('/'));
            result.Friendly.Should().Be(friendly);

            body.Dispose();
        }

        [TestMethod]
        public void Given_Null_When_GetExpanderRequestAsync_Invoked_Then_It_Should_Throw_Exception()
        {
            var req = new Mock<HttpRequest>();

            Func<Task> func = async () => await HttpRequestExtensions.GetExpanderRequestAsync(null, null).ConfigureAwait(false);

            func.Should().Throw<ArgumentNullException>();

            func = async () => await HttpRequestExtensions.GetExpanderRequestAsync(req.Object, null).ConfigureAwait(false);

            func.Should().Throw<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow("POST", "loremipsum")]
        [DataRow("PUT", "loremipsum")]
        [DataRow("DELETE", "loremipsum")]
        [DataRow("PATCH", "loremipsum")]
        [DataRow("HEAD", "loremipsum")]
        [DataRow("OPTION", "loremipsum")]
        public void Given_InvalidMethod_When_GetExpanderRequestAsync_Invoked_Then_It_Should_Throw_Exception(string method, string shortUrl)
        {
            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.Method).Returns(method);

            Func<Task> func = async () => await HttpRequestExtensions.GetExpanderRequestAsync(req.Object, shortUrl).ConfigureAwait(false);

            func.Should().Throw<InvalidOperationException>();
        }

        [DataTestMethod]
        [DataRow("GET", "loremipsum")]
        public async Task Given_Query_When_GetExpanderRequestAsync_Invoked_Then_It_Should_Return_Result(string method, string shortUrl)
        {
            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.Method).Returns(method);

            var result = await HttpRequestExtensions.GetExpanderRequestAsync(req.Object, shortUrl).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.ShortUrl.Should().Be(shortUrl);
        }
    }
}

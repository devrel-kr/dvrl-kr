using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using DevRelKr.UrlShortener.Domains.Exceptions;
using DevRelKr.UrlShortener.Models.Requests;
using DevRelKr.UrlShortener.Models.Responses;
using DevRelKr.UrlShortener.Services;

using FluentAssertions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Newtonsoft.Json;

namespace DevRelKr.UrlShortener.Domains.Tests
{
    [TestClass]
    public class UrlTests
    {
        [TestMethod]
        public void Given_Null_Parameters_When_Initiated_Then_It_Should_Throw_Exception()
        {
            var shortener = new Mock<IShortenerService>();

            Action action = () => new Url(null, null);

            action.Should().Throw<ArgumentNullException>();

            action = () => new Url(shortener.Object, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Given_Null_Request_When_GetRequestAsync_Invoked_Then_It_Should_Throw_Exception()
        {
            var shortener = new Mock<IShortenerService>();
            var expander = new Mock<IExpanderService>();

            var url = new Url(shortener.Object, expander.Object);

            Func<Task> func = async () => await url.GetRequestAsync(null).ConfigureAwait(false);

            func.Should().Throw<ArgumentNullException>();

            func = async () => await url.GetRequestAsync(null, null).ConfigureAwait(false);

            func.Should().Throw<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow("PUT")]
        [DataRow("DELETE")]
        [DataRow("PATCH")]
        public void Given_Request_When_GetRequestAsync_Invoked_Then_It_Should_Throw_Exception(string method)
        {
            var shortener = new Mock<IShortenerService>();
            var expander = new Mock<IExpanderService>();

            var url = new Url(shortener.Object, expander.Object);

            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.Method).Returns(method);

            Func<Task> func = async () => await url.GetRequestAsync(req.Object).ConfigureAwait(false);

            func.Should().Throw<InvalidOperationException>();
        }

        [DataTestMethod]
        [DataRow("GET", null, null)]
        [DataRow("GET", "https://localhost", null)]
        public void Given_RequestWithGet_WithMissingRequiredValues_When_GetRequestAsync_Invoked_Then_It_Should_Throw_Exception(string method, string original, string owner)
        {
            var shortener = new Mock<IShortenerService>();
            var expander = new Mock<IExpanderService>();

            var url = new Url(shortener.Object, expander.Object);

            var dict = new Dictionary<string, StringValues>()
            {
                { "original", original },
                { "owner", owner },
            };
            var collection = new QueryCollection(dict);

            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.Method).Returns(method);
            req.SetupGet(p => p.Query).Returns(collection);

            Func<Task> func = async () => await url.GetRequestAsync(req.Object).ConfigureAwait(false);

            func.Should().Throw<InvalidOperationException>();
        }

        [DataTestMethod]
        [DataRow("GET", "https://localhost", null, null, null, "owner")]
        [DataRow("GET", "https://localhost", "home", null, null, "owner")]
        [DataRow("GET", "https://localhost", "home", "title", null, "owner", "co", "owners")]
        [DataRow("GET", "https://localhost", "home", "title", "description", "owner", "co", "owners")]
        public async Task Given_RequestWithGet_When_GetRequestAsync_Invoked_Then_It_Should_Return_Result(string method, string original, string friendly, string title, string description, string owner, params string[] coOwners)
        {
            var shortener = new Mock<IShortenerService>();
            var expander = new Mock<IExpanderService>();

            var url = new Url(shortener.Object, expander.Object);

            var dict = new Dictionary<string, StringValues>()
            {
                { "original", original },
                { "friendly", friendly },
                { "title", title },
                { "description", description },
                { "owner", owner },
                { "coowners", coOwners },
            };
            var collection = new QueryCollection(dict);

            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.Method).Returns(method);
            req.SetupGet(p => p.Query).Returns(collection);

            var result = await url.GetRequestAsync(req.Object)
                                  .ConfigureAwait(false);

            result.Should().BeOfType<Url>()
                           .And.BeAssignableTo<IUrl>();

            var request = result.ShortenerRequest;
            request.Original.ToString().TrimEnd('/').Should().Be(original.TrimEnd('/'));
            request.Friendly.Should().Be(friendly);
            request.Title.Should().Be(title);
            request.Description.Should().Be(description);
            request.Owner.Should().Be(owner);
            request.CoOwners.Should().BeEquivalentTo(coOwners);
        }

        [DataTestMethod]
        [DataRow("POST", null, null)]
        [DataRow("POST", "https://localhost", null)]
        public void Given_RequestWithPost_When_GetRequestAsync_Invoked_Then_It_Should_Throw_Exception(string method, string original, string owner)
        {
            var shortener = new Mock<IShortenerService>();
            var expander = new Mock<IExpanderService>();

            var url = new Url(shortener.Object, expander.Object);

            var payload = new StringBuilder()
                              .AppendLine("{ ")
                              .Append("  \"Original\": ").Append(string.IsNullOrWhiteSpace(original) ? "null": $"\"{original}\"").AppendLine(",")
                              .Append("  \"Owner\": ").Append(string.IsNullOrWhiteSpace(owner) ? "null": $"\"{owner}\"").AppendLine()
                              .AppendLine("}")
                              .ToString();
            var serialised = Encoding.UTF8.GetBytes(payload);
            var body = new MemoryStream(serialised);

            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.Method).Returns(method);
            req.SetupGet(p => p.Body).Returns(body);

            Func<Task> func = async () => await url.GetRequestAsync(req.Object).ConfigureAwait(false);

            func.Should().Throw<JsonSerializationException>();

            body.Dispose();
        }

        [DataTestMethod]
        [DataRow("POST", "https://localhost", null, null, null, "owner")]
        [DataRow("POST", "https://localhost", "home", null, null, "owner")]
        [DataRow("POST", "https://localhost", "home", "title", null, "owner", "co", "owners")]
        [DataRow("POST", "https://localhost", "home", "title", "description", "owner", "co", "owners")]
        public async Task Given_RequestWithPost_When_GetRequestAsync_Invoked_Then_It_Should_Return_Result(string method, string original, string friendly, string title, string description, string owner, params string[] coOwners)
        {
            var shortener = new Mock<IShortenerService>();
            var expander = new Mock<IExpanderService>();

            var url = new Url(shortener.Object, expander.Object);

            var payload = new ShortenerRequest()
            {
                Original = new Uri(original),
                Friendly = friendly,
                Title = title,
                Description = description,
                Owner = owner,
                CoOwners = coOwners.ToList()
            };
            var serialised = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));
            var body = new MemoryStream(serialised);

            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.Method).Returns(method);
            req.SetupGet(p => p.Body).Returns(body);

            var result = await url.GetRequestAsync(req.Object)
                                  .ConfigureAwait(false);

            result.Should().BeOfType<Url>()
                           .And.BeAssignableTo<IUrl>();

            var request = result.ShortenerRequest;
            request.Original.ToString().TrimEnd('/').Should().Be(original.TrimEnd('/'));
            request.Friendly.Should().Be(friendly);
            request.Title.Should().Be(title);
            request.Description.Should().Be(description);
            request.Owner.Should().Be(owner);
            request.CoOwners.Should().BeEquivalentTo(coOwners);

            body.Dispose();
        }

        [TestMethod]
        public void Given_Null_ShortUrl_When_GetRequestAsync_Invoked_Then_It_Should_Throw_Exception()
        {
            var shortener = new Mock<IShortenerService>();
            var expander = new Mock<IExpanderService>();

            var url = new Url(shortener.Object, expander.Object);

            var req = new Mock<HttpRequest>();

            Func<Task> func = async () => await url.GetRequestAsync(req.Object, null).ConfigureAwait(false);

            func.Should().Throw<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow("POST", "https://dvrl.kr/loremipsum")]
        [DataRow("PUT", "https://dvrl.kr/loremipsum")]
        [DataRow("DELETE", "https://dvrl.kr/loremipsum")]
        [DataRow("PATCH", "https://dvrl.kr/loremipsum")]
        public void Given_Request_With_ShortUrl_When_GetRequestAsync_Invoked_Then_It_Should_Throw_Exception(string method, string shortUrl)
        {
            var shortener = new Mock<IShortenerService>();
            var expander = new Mock<IExpanderService>();

            var url = new Url(shortener.Object, expander.Object);

            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.Method).Returns(method);

            Func<Task> func = async () => await url.GetRequestAsync(req.Object, shortUrl).ConfigureAwait(false);

            func.Should().Throw<InvalidOperationException>();
        }

        [DataTestMethod]
        [DataRow("GET", "loremipsum")]
        public async Task Given_RequestWithGet_With_ShortUrl_When_GetRequestAsync_Invoked_Then_It_Should_Return_Result(string method, string shortUrl)
        {
            var shortener = new Mock<IShortenerService>();
            var expander = new Mock<IExpanderService>();

            var url = new Url(shortener.Object, expander.Object);

            var req = new Mock<HttpRequest>();
            req.SetupGet(p => p.Method).Returns(method);

            var result = await url.GetRequestAsync(req.Object, shortUrl)
                                  .ConfigureAwait(false);

            result.Should().BeOfType<Url>()
                           .And.BeAssignableTo<IUrl>();

            var request = result.ExpanderRequest;
            request.ShortUrl.Should().Be(shortUrl);
        }

        [TestMethod]
        public void Given_Null_Request_When_ValidateAsync_Invoked_Then_It_Should_Throw_Exception()
        {
            var shortener = new Mock<IShortenerService>();
            var expander = new Mock<IExpanderService>();

            var url = new Url(shortener.Object, expander.Object);

            Func<Task> func = async () => await url.ValidateAsync().ConfigureAwait(false);

            func.Should().Throw<InvalidOperationException>();
        }

        [DataTestMethod]
        [DataRow("https://localhost", "home", true)]
        public void Given_Request_When_ValidateAsync_Invoked_Then_It_Should_Throw_Exception(string original, string shortUrl, bool exists)
        {
            var request = new ShortenerRequest()
            {
                Friendly = shortUrl
            };

            var shortener = new Mock<IShortenerService>();
            shortener.Setup(p => p.ExistsAsync(It.IsAny<string>())).ReturnsAsync(exists);

            var expander = new Mock<IExpanderService>();

            var url = new Url(shortener.Object, expander.Object);
            typeof(Url).GetProperty("ShortenerRequest", BindingFlags.Public | BindingFlags.Instance)
                       .SetValue(url, request);

            Func<Task> func = async () => await url.ValidateAsync().ConfigureAwait(false);

            func.Should().Throw<UrlExistsException>()
                .And.ShortUrl.Should().Be(shortUrl);

            url.IsFriendlyUrlValidated.Should().BeFalse();
        }

        [DataTestMethod]
        [DataRow("https://localhost", null, true)]
        [DataRow("https://localhost", "home", false)]
        public async Task Given_Request_When_ValidateAsync_Invoked_Then_It_Should_Return_True(string original, string shortUrl, bool exists)
        {
            var request = new ShortenerRequest()
            {
                Friendly = shortUrl
            };

            var shortener = new Mock<IShortenerService>();
            shortener.Setup(p => p.ExistsAsync(It.IsAny<string>())).ReturnsAsync(exists);

            var expander = new Mock<IExpanderService>();

            var url = new Url(shortener.Object, expander.Object);
            typeof(Url).GetProperty("ShortenerRequest", BindingFlags.Public | BindingFlags.Instance)
                       .SetValue(url, request);

            var result = await url.ValidateAsync().ConfigureAwait(false);

            result.Should().BeOfType<Url>()
                           .And.BeAssignableTo<IUrl>();

            url.IsFriendlyUrlValidated.Should().BeTrue();
        }

        [TestMethod]
        public void Given_Null_Request_When_ShortenAsync_Invoked_Then_It_Should_Throw_Exception()
        {
            var shortener = new Mock<IShortenerService>();
            var expander = new Mock<IExpanderService>();

            var url = new Url(shortener.Object, expander.Object);

            Func<Task> func = async () => await url.ShortenAsync().ConfigureAwait(false);

            func.Should().Throw<InvalidOperationException>();
        }

        [DataTestMethod]
        [DataRow("https://localhost", "home", null, null, "owner", "co", "owners")]
        [DataRow("https://localhost", "home", "title", null, "owner", "co", "owners")]
        [DataRow("https://localhost", "home", "title", "description", "owner", "co", "owners")]
        public async Task Given_Request_When_ShortenAsync_Invoked_Then_It_Should_Return_Result(string original, string shortUrl, string title, string description, string owner, params string[] coOwners)
        {
            var shortened = $"https://dvrl.kr/{(string.IsNullOrWhiteSpace(shortUrl) ? "helloworld" : shortUrl)}";
            var response = new ShortenerResponse()
            {
                Original = new Uri(original),
                Shortened = new Uri(shortened),
                ShortUrl = shortUrl,
                Title = title,
                Description = description,
                Owner = owner,
                CoOwners = coOwners.ToList()
            };
            var shortener = new Mock<IShortenerService>();
            shortener.Setup(p => p.ShortenAsync(It.IsAny<ShortenerRequest>())).ReturnsAsync(response);
            shortener.SetupSequence(p => p.ExistsAsync(It.IsAny<string>()))
                     .ReturnsAsync(true)
                     .ReturnsAsync(false);

            var expander = new Mock<IExpanderService>();

            var request = new ShortenerRequest();

            var url = new Url(shortener.Object, expander.Object);
            typeof(Url).GetProperty("ShortenerRequest", BindingFlags.Public | BindingFlags.Instance)
                       .SetValue(url, request);

            var result = await url.ShortenAsync()
                                  .ConfigureAwait(false);

            result.Should().BeOfType<Url>()
                           .And.BeAssignableTo<IUrl>();

            result.ShortenerResponse.Original.ToString().TrimEnd('/').Should().Be(original.TrimEnd('/'));
            result.ShortenerResponse.Shortened.ToString().TrimEnd('/').Should().Be(shortened.TrimEnd('/'));
            result.ShortenerResponse.ShortUrl.Should().Be(shortUrl);
            result.ShortenerResponse.Title.Should().Be(title);
            result.ShortenerResponse.Description.Should().Be(description);
            result.ShortenerResponse.Owner.Should().Be(owner);
            result.ShortenerResponse.CoOwners.Should().BeEquivalentTo(coOwners);

            url.Original.ToString().TrimEnd('/').Should().Be(original.TrimEnd('/'));
            url.Shortened.ToString().TrimEnd('/').Should().Be(shortened.TrimEnd('/'));
            url.ShortUrl.Should().Be(shortUrl);
            url.Title.Should().Be(title);
            url.Description.Should().Be(description);
            url.Owner.Should().Be(owner);
            url.CoOwners.Should().BeEquivalentTo(coOwners);
        }

        [TestMethod]
        public void Given_Null_Request_When_ExpandAsync_Invoked_Then_It_Should_Throw_Exception()
        {
            var shortener = new Mock<IShortenerService>();
            var expander = new Mock<IExpanderService>();

            var url = new Url(shortener.Object, expander.Object);

            Func<Task> func = async () => await url.ExpandAsync().ConfigureAwait(false);

            func.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public async Task Given_Request_When_ExpandAsync_Invoked_Then_It_Should_Return_Null()
        {
            var shortener = new Mock<IShortenerService>();
            var expander = new Mock<IExpanderService>();
            expander.Setup(p => p.ExpandAsync(It.IsAny<ExpanderRequest>())).ReturnsAsync((ExpanderResponse) null);

            var request = new ExpanderRequest();

            var url = new Url(shortener.Object, expander.Object);
            typeof(Url).GetProperty("ExpanderRequest", BindingFlags.Public | BindingFlags.Instance)
                       .SetValue(url, request);

            var result = await url.ExpandAsync()
                                  .ConfigureAwait(false);

            result.Should().BeOfType<Url>()
                           .And.BeAssignableTo<IUrl>();

            result.ExpanderResponse.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("https://localhost", "home", null, null, "owner", "co", "owners")]
        [DataRow("https://localhost", "home", "title", null, "owner", "co", "owners")]
        [DataRow("https://localhost", "home", "title", "description", "owner", "co", "owners")]
        public async Task Given_Request_When_ExpandAsync_Invoked_Then_It_Should_Return_Result(string original, string shortUrl, string title, string description, string owner, params string[] coOwners)
        {
            var shortener = new Mock<IShortenerService>();

            var shortened = $"https://dvrl.kr/{(string.IsNullOrWhiteSpace(shortUrl) ? "helloworld" : shortUrl)}";
            var response = new ExpanderResponse()
            {
                Original = new Uri(original),
                Shortened = new Uri(shortened),
                ShortUrl = shortUrl,
                Title = title,
                Description = description,
                Owner = owner,
                CoOwners = coOwners.ToList()
            };

            var expander = new Mock<IExpanderService>();
            expander.Setup(p => p.ExpandAsync(It.IsAny<ExpanderRequest>())).ReturnsAsync(response);

            var request = new ExpanderRequest();

            var url = new Url(shortener.Object, expander.Object);
            typeof(Url).GetProperty("ExpanderRequest", BindingFlags.Public | BindingFlags.Instance)
                       .SetValue(url, request);

            var result = await url.ExpandAsync()
                                  .ConfigureAwait(false);

            result.Should().BeOfType<Url>()
                           .And.BeAssignableTo<IUrl>();

            result.ExpanderResponse.Should().NotBeNull();
            result.ExpanderResponse.Original.ToString().TrimEnd('/').Should().Be(original.TrimEnd('/'));
            result.ExpanderResponse.Shortened.ToString().TrimEnd('/').Should().Be(shortened.TrimEnd('/'));
            result.ExpanderResponse.ShortUrl.Should().Be(shortUrl);
            result.ExpanderResponse.Title.Should().Be(title);
            result.ExpanderResponse.Description.Should().Be(description);
            result.ExpanderResponse.Owner.Should().Be(owner);
            result.ExpanderResponse.CoOwners.Should().BeEquivalentTo(coOwners);

            url.Original.ToString().TrimEnd('/').Should().Be(original.TrimEnd('/'));
            url.Shortened.ToString().TrimEnd('/').Should().Be(shortened.TrimEnd('/'));
            url.ShortUrl.Should().Be(shortUrl);
            url.Title.Should().Be(title);
            url.Description.Should().Be(description);
            url.Owner.Should().Be(owner);
            url.CoOwners.Should().BeEquivalentTo(coOwners);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(100)]
        public async Task Given_ShortenerResponse_When_AddHitCountAsync_Invoked_Then_It_Should_Return_Result(int hitCount)
        {
            var shortener = new Mock<IShortenerService>();
            var expander = new Mock<IExpanderService>();
            var response = new ShortenerResponse() { HitCount = hitCount };

            var url = new Url(shortener.Object, expander.Object);
            typeof(Url).GetProperty("ShortenerResponse", BindingFlags.Public | BindingFlags.Instance)
                       .SetValue(url, response);
            typeof(Url).GetProperty("HitCount", BindingFlags.Public | BindingFlags.Instance)
                       .SetValue(url, hitCount);

            var result = await url.AddHitCountAsync<ShortenerResponse>().ConfigureAwait(false);

            result.Should().BeOfType<Url>()
                           .And.BeAssignableTo<IUrl>();

            url.HitCount.Should().Be(hitCount + 1);
            url.ShortenerResponse.HitCount.Should().Be(hitCount + 1);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(100)]
        public async Task Given_ExpanderResponse_When_AddHitCountAsync_Invoked_Then_It_Should_Return_Result(int hitCount)
        {
            var shortener = new Mock<IShortenerService>();
            var expander = new Mock<IExpanderService>();
            var response = new ExpanderResponse() { HitCount = hitCount };

            var url = new Url(shortener.Object, expander.Object);
            typeof(Url).GetProperty("ExpanderResponse", BindingFlags.Public | BindingFlags.Instance)
                       .SetValue(url, response);
            typeof(Url).GetProperty("HitCount", BindingFlags.Public | BindingFlags.Instance)
                       .SetValue(url, hitCount);

            var result = await url.AddHitCountAsync<ExpanderResponse>().ConfigureAwait(false);

            result.Should().BeOfType<Url>()
                           .And.BeAssignableTo<IUrl>();

            url.HitCount.Should().Be(hitCount + 1);
            url.ExpanderResponse.HitCount.Should().Be(hitCount + 1);
        }

        [TestMethod]
        public async Task Given_Response_When_CreateRecordAsync_Invoked_Then_It_Should_Return_Result()
        {
            var shortener = new Mock<IShortenerService>();
            var expander = new Mock<IExpanderService>();
            var response = new ShortenerResponse();

            var url = new Url(shortener.Object, expander.Object);
            typeof(Url).GetProperty("ShortenerResponse", BindingFlags.Public | BindingFlags.Instance)
                       .SetValue(url, response);

            var utcNow = DateTimeOffset.UtcNow;
            var entityId = Guid.NewGuid();

            var result = await url.CreateRecordAsync(utcNow, entityId)
                                  .ConfigureAwait(false);

            result.Should().BeOfType<Url>()
                           .And.BeAssignableTo<IUrl>();

            url.DateGenerated.Should().Be(utcNow);
            url.DateUpdated.Should().Be(utcNow);
        }

        [TestMethod]
        public async Task Given_ShortenerResponse_When_UpdateRecordAsync_Invoked_Then_It_Should_Return_Result()
        {
            var utcNow = DateTimeOffset.UtcNow;

            var shortener = new Mock<IShortenerService>();
            var expander = new Mock<IExpanderService>();
            var response = new ShortenerResponse();
            response.DateGenerated = utcNow;
            response.DateUpdated = utcNow;

            var url = new Url(shortener.Object, expander.Object);
            typeof(Url).GetProperty("ShortenerResponse", BindingFlags.Public | BindingFlags.Instance)
                       .SetValue(url, response);
            typeof(Url).GetProperty("DateGenerated", BindingFlags.Public | BindingFlags.Instance)
                       .SetValue(url, utcNow);

            var result = await url.UpdateRecordAsync<ShortenerResponse>(utcNow)
                                  .ConfigureAwait(false);

            result.Should().BeOfType<Url>()
                           .And.BeAssignableTo<IUrl>();

            url.DateGenerated.Should().Be(utcNow);
            url.DateUpdated.Should().Be(utcNow);
        }

        [TestMethod]
        public async Task Given_ExpanderResponse_When_UpdateRecordAsync_Invoked_Then_It_Should_Return_Result()
        {
            var utcNow = DateTimeOffset.UtcNow;

            var shortener = new Mock<IShortenerService>();
            var expander = new Mock<IExpanderService>();
            var response = new ExpanderResponse();
            response.DateGenerated = utcNow;
            response.DateUpdated = utcNow;

            var url = new Url(shortener.Object, expander.Object);
            typeof(Url).GetProperty("ExpanderResponse", BindingFlags.Public | BindingFlags.Instance)
                       .SetValue(url, response);
            typeof(Url).GetProperty("DateGenerated", BindingFlags.Public | BindingFlags.Instance)
                       .SetValue(url, utcNow);

            var result = await url.UpdateRecordAsync<ExpanderResponse>(utcNow)
                                  .ConfigureAwait(false);

            result.Should().BeOfType<Url>()
                           .And.BeAssignableTo<IUrl>();

            url.DateGenerated.Should().Be(utcNow);
            url.DateUpdated.Should().Be(utcNow);
        }
    }
}

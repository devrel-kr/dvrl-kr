using System;

using DevRelKr.UrlShortener.Models.Requests;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

namespace DevRelKr.UrlShortener.Models.Tests.Requests
{
    [TestClass]
    public class ShortenerRequestTests
    {
        [TestMethod]
        public void Given_Payload_Without_Original_When_Serialised_Then_It_Should_Throw_Exception()
        {
            var payload = new ShortenerRequest()
            {
                Friendly = "lorem ipsum"
            };

            Action action = () => JsonConvert.SerializeObject(payload);

            action.Should().Throw<JsonSerializationException>();
        }

        [TestMethod]
        public void Given_Payload_Without_Original_When_Deserialised_Then_It_Should_Throw_Exception()
        {
            var payload = "{ \"friendly\": \"lorem ipsum\" }";

            Action action = () => JsonConvert.DeserializeObject<ShortenerRequest>(payload);

            action.Should().Throw<JsonSerializationException>();
        }

        [TestMethod]
        public void Given_Payload_Without_Owner_When_Serialised_Then_It_Should_Throw_Exception()
        {
            var payload = new ShortenerRequest()
            {
                Original = new Uri("https://localhost"),
                Friendly = "lorem ipsum"
            };

            Action action = () => JsonConvert.SerializeObject(payload);

            action.Should().Throw<JsonSerializationException>();
        }

        [TestMethod]
        public void Given_Payload_Without_Owner_When_Deserialised_Then_It_Should_Throw_Exception()
        {
            var payload = "{ \"original\": \"https://localhost\", \"friendly\": \"lorem ipsum\" }";

            Action action = () => JsonConvert.DeserializeObject<ShortenerRequest>(payload);

            action.Should().Throw<JsonSerializationException>();
        }
    }
}

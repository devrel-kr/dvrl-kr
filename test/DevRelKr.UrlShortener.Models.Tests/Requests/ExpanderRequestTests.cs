using System;

using DevRelKr.UrlShortener.Models.Requests;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

namespace DevRelKr.UrlShortener.Models.Tests.Requests
{
    [TestClass]
    public class ExpanderRequestTests
    {
        [TestMethod]
        public void Given_Payload_Without_Original_When_Serialised_Then_It_Should_Throw_Exception()
        {
            var payload = new ExpanderRequest();

            Action action = () => JsonConvert.SerializeObject(payload);

            action.Should().Throw<JsonSerializationException>();
        }

        [TestMethod]
        public void Given_Payload_Without_Original_When_Deserialised_Then_It_Should_Throw_Exception()
        {
            var payload = "{ }";

            Action action = () => JsonConvert.DeserializeObject<ShortenerRequest>(payload);

            action.Should().Throw<JsonSerializationException>();
        }
    }
}

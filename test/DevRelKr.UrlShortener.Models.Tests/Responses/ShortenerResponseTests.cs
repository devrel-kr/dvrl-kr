using DevRelKr.UrlShortener.Models.Responses;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevRelKr.UrlShortener.Models.Tests.Responses
{
    [TestClass]
    public class ShortenerResponseTests
    {
        [TestMethod]
        public void Given_Type_When_Instantiated_Then_It_Should_Return_Result()
        {
            var entity = new ShortenerResponse();

            entity.CoOwners.Should().NotBeNull()
                           .And.HaveCount(0);
        }
    }
}

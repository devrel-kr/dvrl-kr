using DevRelKr.UrlShortener.Tests.Fakes;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevRelKr.UrlShortener.Models.Tests.DataStores
{
    [TestClass]
    public class ItemEntityCollectionTests
    {
        [TestMethod]
        public void Given_Type_When_Instantiated_Then_It_Should_Return_Result()
        {
            var entity = new FakeItemEntityCollection();

            entity.Items.Should().NotBeNull()
                        .And.HaveCount(0);
        }
    }
}

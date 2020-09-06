using DevRelKr.UrlShortener.Models.DataStores;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevRelKr.UrlShortener.Models.Tests.DataStores
{
    [TestClass]
    public class UrlItemEntityCollectionTests
    {
        [TestMethod]
        public void Given_Type_When_Instantiated_Then_It_Should_Return_Result()
        {
            var entity = new UrlItemEntityCollection();

            entity.Items.Should().NotBeNull()
                        .And.HaveCount(0);
        }
    }
}

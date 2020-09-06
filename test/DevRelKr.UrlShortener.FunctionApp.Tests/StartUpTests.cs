using DevRelKr.UrlShortener.Domains;
using DevRelKr.UrlShortener.Models.Configurations;
using DevRelKr.UrlShortener.Services;

using FluentAssertions;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace DevRelKr.UrlShortener.FunctionApp.Tests
{
    [TestClass]
    public class StartUpTests
    {
        // [TestMethod]
        // public void Given_Dependency_When_Configure_Invoked_THen_It_Should_Return_Result()
        // {
        //     var services = new ServiceCollection();

        //     var builder = new Mock<IFunctionsHostBuilder>();
        //     builder.SetupGet(p => p.Services).Returns(services);

        //     var instance = new StartUp();

        //     instance.Configure(builder.Object);

        //     services.Should().Contain(p => p.ServiceType == typeof(AppSettings));
        //     services.Should().Contain(p => p.ServiceType == typeof(IShortenerService));
        //     services.Should().Contain(p => p.ServiceType == typeof(IUrl));
        // }
    }
}

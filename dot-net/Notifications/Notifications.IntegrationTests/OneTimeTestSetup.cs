using System.Net.Http;
using NUnit.Framework;

namespace Notifications.IntegrationTests
{
    [SetUpFixture]
    public static class OneTimeTestSetup
    {
        public static HttpClient Client;
        public static ApiWebApplicationFactory<Startup> Factory;

        [OneTimeSetUp]
        public static void GivenARequestToTheController()
        {
            Factory = new ApiWebApplicationFactory<Startup>();
            Client = Factory.CreateClient();
        }

        [OneTimeTearDown]
        public static void TearDown()
        {
            Client?.Dispose();
            Factory?.Dispose();
        }
    }
}
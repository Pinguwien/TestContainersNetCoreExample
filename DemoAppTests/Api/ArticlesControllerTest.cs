using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DemoApp.Bootstrap;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace DemoAppTests.Api
{
    public class ArticlesControllerTest
    {
        [Test]
        public async Task SucceedsWhenGetRequestReturnsListOfArticles()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {
                    "DB:ConnectionString",
                    "User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=demo-db;Pooling=true;"
                }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var webHostBuilder = new WebHostBuilder().UseConfiguration(configuration).UseStartup<Startup>();
            using var server = new TestServer(webHostBuilder);
            using var client = server.CreateClient();
            const string url = "https://localhost:5001/api/Articles";

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        /*[Test]
        public async Task SucceedsWhenCallToProtectedResourceWithoutTokenReturns401()
        {
        }

        [Test]
        public async Task SucceedsWhenCallToProtectedResourceWithTokenReturnsArticles()
        {
        }*/
    }
}
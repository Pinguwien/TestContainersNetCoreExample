using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
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
        private readonly Dictionary<string, string> _testConfig = new Dictionary<string, string>
        {
            {
                "DB:ConnectionString",
                "User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=demo-db;Pooling=true;"
            },
            {
                "Jwt:Audience",
                "account"
            },
            {
                "Jwt:Issuer",
                "http://" + BaseFixture.KeycloakContainer.Hostname + ":8080/auth/realms/example"
            }
        };

        [Test]
        public async Task SucceedsWhenGetRequestReturnsListOfArticles()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(_testConfig)
                .Build();

            var webHostBuilder = new WebHostBuilder().UseConfiguration(configuration).UseStartup<Startup>();
            using var server = new TestServer(webHostBuilder);
            using var client = server.CreateClient();
            const string url = "https://localhost:5001/api/Articles";

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task SucceedsWhenGetRequestOnProtectedResourceReturnsUnauthorizedWithoutToken()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(_testConfig)
                .Build();

            var webHostBuilder = new WebHostBuilder().UseConfiguration(configuration).UseStartup<Startup>();
            using var server = new TestServer(webHostBuilder);
            using var client = server.CreateClient();
            const string url = "https://localhost:5001/api/Articles/Protected";

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public async Task SucceedsWhenGetRequestWithValidAuthorizationHeaderReturnsListOfArticles()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(_testConfig)
                .Build();

            var webHostBuilder = new WebHostBuilder().UseConfiguration(configuration).UseEnvironment("Testing")
                .UseStartup<Startup>();

            using var server = new TestServer(webHostBuilder);
            using var client = server.CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", BaseFixture.TestToken);

            const string url = "https://localhost:5001/api/Articles/Protected";

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
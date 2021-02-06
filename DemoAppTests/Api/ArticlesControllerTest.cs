using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DemoApp.Bootstrap;
using DemoApp.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                "https://" + BaseFixture.KeycloakContainer.Hostname + ":8443/auth/realms/example"
            }
        };

        private const string OpenEndpoint = "https://localhost:5001/api/Articles";

        private const string ProtectedEndpoint = "https://localhost:5001/api/Articles/Protected";

        [Test]
        public async Task SucceedsWhenGetRequestReturnsListOfArticles()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(_testConfig)
                .Build();

            var webHostBuilder = new WebHostBuilder().UseConfiguration(configuration).UseStartup<Startup>();
            using var server = new TestServer(webHostBuilder);
            using var client = server.CreateClient();

            var response = await client.GetAsync(OpenEndpoint);
            var result = JsonConvert.DeserializeObject<List<Article>>(await response.Content.ReadAsStringAsync());            
            
            Assert.That(result.First().Name, Is.EqualTo("article1"));
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

            var response = await client.GetAsync(ProtectedEndpoint);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test]
        public async Task SucceedsWhenGetRequestWithTokenReturnsListOfArticles()
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


            var response = await client.GetAsync(ProtectedEndpoint);
            var result = JsonConvert.DeserializeObject<List<Article>>(await response.Content.ReadAsStringAsync());            
            
            Assert.That(result.First().Name, Is.EqualTo("article1"));
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        
        //error on middleware backchannel request to identity server for getting configuration
        //when self-signed certs are not supported in env. 
        [Test]
        public void SucceedsWhenInvalidOperationExceptionIsThrownWhenNotInTestingEnvironment()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(_testConfig)
                .Build();

            var webHostBuilder = new WebHostBuilder().UseConfiguration(configuration)
                .UseStartup<Startup>();

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                using var server = new TestServer(webHostBuilder);
                using var client = server.CreateClient();

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", BaseFixture.TestToken);
                await client.GetAsync(ProtectedEndpoint);
            });

        }
    }
}
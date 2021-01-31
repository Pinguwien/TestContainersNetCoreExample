using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.OutputConsumers;
using DotNet.Testcontainers.Containers.WaitStrategies;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace DemoAppTests.Infrastructure.Persistence
{
    public class KeycloakTest
    {
        private static TestcontainersContainer KeycloakContainer { get; set; }

        private const string UnixSocketAddr = "unix:/var/run/docker.sock";
        private const string KeycloakWaitLogMsg = ".*services are lazy, passive or on-demand.*\\n";

        private readonly string _importPath =
            Path.GetDirectoryName(
                Path.GetDirectoryName(Path.GetDirectoryName(TestContext.CurrentContext.TestDirectory)));

        [SetUp]
        public async Task SetUp()
        {
            var dockerEndpoint = Environment.GetEnvironmentVariable("DOCKER_HOST") ?? UnixSocketAddr;
            using var consumer = Consume.RedirectStdoutAndStderrToStream(new MemoryStream(), new MemoryStream());

            var keycloakContainerBuilder = new TestcontainersBuilder<TestcontainersContainer>()
                .WithDockerEndpoint(dockerEndpoint)
                .WithImage("jboss/keycloak:12.0.1")
                .WithName("tc-Keycloak")
                .WithPortBinding(8080)
                .WithOutputConsumer(consumer)
                .WithMount(_importPath +
                           "/example-realm2.json",
                    "/tmp/example-realm.json")
                .WithCommand("-c standalone.xml",
                    "-b 0.0.0.0",
                    "-Dkeycloak.profile.feature.upload_scripts=enabled")
                .WithEnvironment("KEYCLOAK_USER", "admin")
                .WithEnvironment("KEYCLOAK_PASSWORD", "admin")
                .WithEnvironment("KEYCLOAK_IMPORT",
                    "/tmp/example-realm.json")
                .WithWaitStrategy(
                    Wait.ForUnixContainer()
                        .UntilPortIsAvailable(8080)
                        .UntilMessageIsLogged(consumer.Stdout, KeycloakWaitLogMsg))
                .WithCleanUp(true);

            KeycloakContainer = keycloakContainerBuilder.Build();
            await KeycloakContainer.StartAsync();
        }

        [Test]
        public async Task SucceedsWhenAccessTokenIsRequestedAndCanBeRead()
        {
            var url = "http://localhost:8080/auth/realms/example/protocol/openid-connect/token";
            var testParams = new Dictionary<string, string>();
            testParams.Add("client_id", "demoClient");
            testParams.Add("grant_type", "password");
            testParams.Add("username", "user");
            testParams.Add("password", "password");

            var client = new HttpClient();
            var response = await client.PostAsync(url, new FormUrlEncodedContent(testParams));
            Assert.That(response.IsSuccessStatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var json = (JObject)JsonConvert.DeserializeObject(content);
            string tokenString = json["access_token"].Value<string>();
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);
            var username = token.Claims.First(claim => claim.Type == "preferred_username").Value;
            Assert.That(username, Is.EqualTo("user"));
        }

        [TearDown]
        public async Task TearDown()
        {
            await KeycloakContainer.StopAsync();
            await KeycloakContainer.DisposeAsync();
        }
    }

    class TokenContainer
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        
        [JsonProperty("refresh_expires_in")]
        public int RefreshExpiresIn { get; set; }
        
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        
        [JsonProperty("not-before-policy")]
        public int NotBeforePolicy { get; set; }
        
        [JsonProperty("session_state")]
        public string SessionState { get; set; }
        
        [JsonProperty("scope")]
        public string Scope { get; set; }
    }
}
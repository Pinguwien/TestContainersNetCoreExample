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
                .WithPortBinding(8443)
                .WithOutputConsumer(consumer)
                .WithMount(_importPath +
                           "/example-realm.json",
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
                        .UntilPortIsAvailable(8443)
                        .UntilMessageIsLogged(consumer.Stdout, KeycloakWaitLogMsg))
                .WithCleanUp(true);

            KeycloakContainer = keycloakContainerBuilder.Build();
            await KeycloakContainer.StartAsync();
        }

        [Test]
        public async Task SucceedsWhenAccessTokenIsRequestedAndCanBeRead()
        {
            var url = "https://"+ KeycloakContainer.Hostname +":8443"+"/auth/realms/example/protocol/openid-connect/token";
            var testParams = new Dictionary<string, string>
            {
                {"client_id", "demoClient"},
                {"grant_type", "password"},
                {"username", "user"},
                {"password", "password"}
            };

            //Apparently, I need this in TestServers HttpClient.
            using var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using var client = new HttpClient(httpClientHandler);

            var response = await client.PostAsync(url, new FormUrlEncodedContent(testParams));
            Assert.That(response.IsSuccessStatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var json = (JObject) JsonConvert.DeserializeObject(content);
            var tokenString = json["access_token"].Value<string>();
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
}
using System;
using System.IO;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.OutputConsumers;
using DotNet.Testcontainers.Containers.WaitStrategies;
using NUnit.Framework;

namespace DemoAppTests.Infrastructure.Persistence
{
    public class KeycloakTest
    {
        public static TestcontainersContainer KeycloakContainer { get; private set; }

        private const string UnixSocketAddr = "unix:/var/run/docker.sock";
        private const string WaitLogMsg = ".*services are lazy, passive or on-demand.*\\n";

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
                        .UntilMessageIsLogged(consumer.Stdout, WaitLogMsg))
                .WithCleanUp(true);

            KeycloakContainer = keycloakContainerBuilder.Build();
            await KeycloakContainer.StartAsync();
        }

        [Test]
        public async Task TempTestIfKeycloakStarted()
        {
            //senseless for now, just to check via debug if it starts correctly when Message is logged via debug mode for now.
            var expected = 0;
            int i = 0;
            Assert.That(i.Equals(expected));
        }

        [TearDown]
        public async Task TearDown()
        {
            await KeycloakContainer.StopAsync();
            await KeycloakContainer.DisposeAsync();
        }
    }
}
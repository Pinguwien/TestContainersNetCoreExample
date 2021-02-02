using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.Modules.Databases;
using DotNet.Testcontainers.Containers.OutputConsumers;
using DotNet.Testcontainers.Containers.WaitStrategies;
using Npgsql;
using NUnit.Framework;

namespace DemoAppTests.Infrastructure.Persistence.DbContainerForMultipleTestsSetup
{
    [SetUpFixture]
    public class BaseFixture
    {
        private const string PathToMigrations = "../../../../DemoApp/Infrastructure/Persistence/Migrations";
        private const string UnixSocketAddr = "unix:/var/run/docker.sock";
        private const string PathToTestData = "../../../TestData/";
        private const string KeycloakWaitLogMsg = ".*services are lazy, passive or on-demand.*\\n";

        private readonly string _importPath =
            Path.GetDirectoryName(
                Path.GetDirectoryName(Path.GetDirectoryName(TestContext.CurrentContext.TestDirectory)));

        public static PostgreSqlTestcontainer PostgresContainer { get; private set; }
        public static TestcontainersContainer KeycloakContainer { get; private set; }

        // use e.g. docker stats to have a live view of running containers
        [OneTimeSetUp]
        public async Task SetupOnce()
        {
            var dockerEndpoint = Environment.GetEnvironmentVariable("DOCKER_HOST") ?? UnixSocketAddr;

            var postgresContainerBuilder = new TestcontainersBuilder<PostgreSqlTestcontainer>()
                .WithDockerEndpoint(dockerEndpoint)
                .WithDatabase(new PostgreSqlTestcontainerConfiguration
                {
                    Database = "demo-db",
                    Username = "postgres",
                    Password = "postgres",
                })
                .WithImage("postgres:13.1-alpine")
                .WithName("tc-Postgres")
                .WithCleanUp(true);

            PostgresContainer = postgresContainerBuilder.Build();

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
            await PostgresContainer.StartAsync();

            FillDb();
        }

        private void FillDb()
        {
            var conn = new NpgsqlConnection(PostgresContainer.ConnectionString);

            var evolve = new Evolve.Evolve(conn, msg => Debug.WriteLine(msg))
            {
                Locations = new[] {PathToMigrations, PathToTestData}
            };
            evolve.Migrate();
        }

        [OneTimeTearDown]
        public async Task TeardownOnce()
        {
            await KeycloakContainer.StopAsync();
            await KeycloakContainer.DisposeAsync();

            await PostgresContainer.StopAsync();
            await PostgresContainer.DisposeAsync(); //important for the event to cleanup to be fired!
        }
    }
}
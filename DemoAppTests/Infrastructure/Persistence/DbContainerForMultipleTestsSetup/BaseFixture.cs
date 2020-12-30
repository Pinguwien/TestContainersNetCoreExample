using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Databases;
using Npgsql;
using NUnit.Framework;

namespace DemoAppTests.Infrastructure.Persistence.DbContainerForMultipleTestsSetup
{
    [SetUpFixture]
    public class BaseFixture
    {
        public static PostgreSqlTestcontainer PostgresContainer { get; private set; }

        //TODO: fährt hoch aber realmimport nich möglich. cliscript evtl? aber alles unschön ohne wait.for(http) mit auth.
        //public static TestcontainersContainer KeycloakContainer { get; private set; }

        // docker stats -> liveansicht
        [OneTimeSetUp]
        public async Task SetupOnce()
        {
            var dockerEndpoint = Environment.GetEnvironmentVariable("DOCKER_HOST");
            var fallback = "unix:/var/run/docker.sock";

            var postgresContainerBuilder = new TestcontainersBuilder<PostgreSqlTestcontainer>()
                .WithDockerEndpoint(dockerEndpoint ?? fallback)
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

            //TODO
            /*var keycloakContainerBuilder = new TestcontainersBuilder<TestcontainersContainer>()
                .WithDockerEndpoint(dockerEndpoint)
                .WithImage("jboss/keycloak:12.0.1")
                .WithName("tc-Keycloak")
                .WithMount("/Users/dguhr/git/TestContainersDemo/DemoAppTests/example-realm.json", "/tmp/example-realm.json")
                .WithCommand("-c standalone.xml", // don't start infinispan cluster
                    "-b 0.0.0.0", // ensure proper binding
                    "-Dkeycloak.profile.feature.upload_scripts=enabled")
                .WithPortBinding(8080)
                //.WithEnvironment("KEYCLOAK_USER", "admin")
                //.WithEnvironment("KEYCLOAK_PASSWORD", "admin")
                .WithEnvironment("KEYCLOAK_IMPORT", "/Users/dguhr/git/TestContainersDemo/DemoAppTests/example-realm.json")
                .WithWaitStrategy(
                    Wait.ForUnixContainer()
                        .UntilFileExists("/tmp/example-realm.json")
                        .UntilPortIsAvailable(8080))
                .WithCleanUp(true);
            
            KeycloakContainer = keycloakContainerBuilder.Build();*/

            await PostgresContainer.StartAsync();
            //await KeycloakContainer.StartAsync();
            //var client = new HttpClient();

            //var resp = await client.GetAsync("http://localhost:8080/auth");


            FillDb();
        }

        private void FillDb()
        {
            var conn = new NpgsqlConnection(PostgresContainer.ConnectionString);

            var evolve = new Evolve.Evolve(conn, msg => Debug.WriteLine(msg))
            {
                Locations = new[] {"../../../../DemoApp/Infrastructure/Persistence/Migrations", "../../../TestData/"}
            };
            evolve.Migrate();
        }

        [OneTimeTearDown]
        public async Task TeardownOnce()
        {
            await PostgresContainer.StopAsync();
            await PostgresContainer.DisposeAsync(); //important for the event to cleanup!

            //await KeycloakContainer.StopAsync();
            //await KeycloakContainer.DisposeAsync();
        }
    }
}
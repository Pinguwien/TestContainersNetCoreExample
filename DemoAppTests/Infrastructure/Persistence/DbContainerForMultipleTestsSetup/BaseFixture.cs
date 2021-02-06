using System;
using System.Diagnostics;
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
        private const string PathToMigrations = "../../../../DemoApp/Infrastructure/Persistence/Migrations";
        private const string UnixSocketAddr = "unix:/var/run/docker.sock";
        private const string PathToTestData = "../../../TestData/";

        public static PostgreSqlTestcontainer PostgresContainer { get; private set; }

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
            await PostgresContainer.StopAsync();
            await PostgresContainer.DisposeAsync(); //important for the event to cleanup to be fired!
        }
    }
}
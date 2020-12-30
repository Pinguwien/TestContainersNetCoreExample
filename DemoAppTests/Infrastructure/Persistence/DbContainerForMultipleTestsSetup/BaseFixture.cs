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
        private static string _pathToMigrations = "../../../../DemoApp/Infrastructure/Persistence/Migrations";
        private static string _unixSocketAddr = "unix:/var/run/docker.sock";
        private static string _pathToTestData = "../../../TestData/";
        public static PostgreSqlTestcontainer PostgresContainer { get; private set; }

        // use e.g. docker stats to have a live view of running containers
        [OneTimeSetUp]
        public async Task SetupOnce()
        {
            var dockerEndpoint = Environment.GetEnvironmentVariable("DOCKER_HOST") ?? _unixSocketAddr;
            
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
                Locations = new[] {_pathToMigrations, _pathToTestData}
            };
            evolve.Migrate();
        }

        [OneTimeTearDown]
        public async Task TeardownOnce()
        {
            await PostgresContainer.StopAsync();
            await PostgresContainer.DisposeAsync(); //important for the event to cleanup!
        }
    }
}
using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Databases;
using Npgsql;
using NUnit.Framework;

namespace DemoAppTests.Infrastructure.Persistence
{
    [TestFixture]
    public class MyFirstIntTest
    {
        private PostgreSqlTestcontainer PostgresContainer { get; set; }
        private static string _unixSocketAddr = "unix:/var/run/docker.sock";

        [SetUp]
        public async Task SetUp()
        {
            var dockerEndpoint = Environment.GetEnvironmentVariable("DOCKER_HOST") ?? _unixSocketAddr;

            var postgresBuilder = new TestcontainersBuilder<PostgreSqlTestcontainer>()
                .WithDockerEndpoint(dockerEndpoint)
                .WithDatabase(new PostgreSqlTestcontainerConfiguration
                {
                    Database = "demo-db",
                    Username = "postgres",
                    Password = "postgres",
                })
                .WithImage("postgres:13.1-alpine")
                .WithCleanUp(true);

            PostgresContainer = postgresBuilder.Build();

            await PostgresContainer.StartAsync();
        }

        [Test]
        public void GivenTestcontainersIsInstalledCheckIfTheContainerIsRunning()
        {
            using var connection = new NpgsqlConnection(PostgresContainer.ConnectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand {Connection = connection, CommandText = "SELECT 1"};
            cmd.ExecuteReader();
        }

        //only for showing via docker stats-command that another container is used.
        [Test]
        public void GivenTestcontainersIsInstalledCheckIfDifferentContainerIsRunningSecondForSecondTest()
        {
            using var connection = new NpgsqlConnection(PostgresContainer.ConnectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand {Connection = connection, CommandText = "SELECT 1"};
            cmd.ExecuteReader();
        }

        [TearDown]
        public async Task TearDown()
        {
            await PostgresContainer.StopAsync();
            await PostgresContainer.DisposeAsync();
        }
    }
}
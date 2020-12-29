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
        public PostgreSqlTestcontainer PostgresContainer { get;  set; }

        [SetUp]
        public async Task SetUp()
        {
            var fallback = "unix:/var/run/docker.sock";
            var dockerEndpoint = Environment.GetEnvironmentVariable("DOCKER_HOST");
            var testcontainersBuilder = new TestcontainersBuilder<PostgreSqlTestcontainer>()
                .WithDockerEndpoint(dockerEndpoint ?? fallback)
                .WithDatabase(new PostgreSqlTestcontainerConfiguration
                {
                    Database = "demo-db",
                    Username = "postgres",
                    Password = "postgres",
                })
                .WithImage("postgres:13.1-alpine").WithCleanUp(true);

            PostgresContainer = testcontainersBuilder.Build();
            
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

        //only for showing via docker stats that another container is used.
        [Test]
        public void GivenTestcontainersIsInstalledCheckIfTheContainerIsRunningSecondTime()
        {
            using var connection = new NpgsqlConnection(PostgresContainer.ConnectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand {Connection = connection, CommandText = "SELECT 1"};
            cmd.ExecuteReader();
        }
        
        // docker stats -> liveansicht 
        [TearDown]
        public async Task TearDown()
        {
            await PostgresContainer.StopAsync();
            await PostgresContainer.DisposeAsync();
        }
    }
}
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DemoApp.Domain;
using DemoApp.Infrastructure.Persistence;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Databases;
using Npgsql;
using NUnit.Framework;

namespace DemoAppTests.Infrastructure.Persistence
{
    public class DapperArticlesRepositoryIntTest
    {
        private PostgreSqlTestcontainer PostgresContainer { get; set; }
        private static string _pathToMigrations = "../../../../DemoApp/Infrastructure/Persistence/Migrations";
        private static string _unixSocketAddr = "unix:/var/run/docker.sock";
        private static string _pathToTestData = "../../../TestData/";

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
                .WithName("tc-dedicated-postgres")
                .WithCleanUp(true);

            PostgresContainer = postgresBuilder.Build();

            await PostgresContainer.StartAsync();

            FillDb();
        }

        [Test]
        public async Task SucceedsWhenArticleGetsInsertedCorrectly()
        {
            var mockArticle = new Article()
            {
                Name = "MockArticle",
                Description = "Mocked By Integration Test",
                Price = 99.99m,
                Created = DateTimeOffset.Now
            };
            var cut = new DapperArticlesRepository(
                new NpgsqlConnection(PostgresContainer.ConnectionString));

            await cut.CreateNewArticle(mockArticle);

            var created = cut.GetArticlesByName("MockArticle").Result.First();
            Assert.That(created, Is.Not.Null);
            Assert.That(created.Name, Is.EqualTo(mockArticle.Name));
        }

        [TearDown]
        public async Task TearDown()
        {
            await PostgresContainer.StopAsync();
            await PostgresContainer.DisposeAsync();
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
    }
}
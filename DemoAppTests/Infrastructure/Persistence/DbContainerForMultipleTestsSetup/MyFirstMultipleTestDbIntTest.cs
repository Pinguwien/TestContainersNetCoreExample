using Npgsql;
using NUnit.Framework;

namespace DemoAppTests.Infrastructure.Persistence.DbContainerForMultipleTestsSetup
{
    public class MyFirstMultipleTestDbIntTest
    {
        [Test]
        public void SucceedsWhenAccessToPostgresTestcontainerIsGenerallyGiven()
        {
            using var connection = new NpgsqlConnection(BaseFixture.PostgresContainer.ConnectionString);
            connection.Open();
            using var cmd = new NpgsqlCommand {Connection = connection, CommandText = "SELECT 1"};
            cmd.ExecuteReader();
            connection.Close();
        }

        //only tto demonstrate that it is the same container with the fixture. see docker stats.
        [Test]
        public void SucceedsWhenSameContainerIsStillRunningForMoreThanOneTest()
        {
            using var connection = new NpgsqlConnection(BaseFixture.PostgresContainer.ConnectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand {Connection = connection, CommandText = "SELECT 1"};
            cmd.ExecuteReader();
            connection.Close();
        }
    }
}
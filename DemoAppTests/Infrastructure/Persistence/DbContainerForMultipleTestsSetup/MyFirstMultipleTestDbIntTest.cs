using System;
using System.Linq;
using System.Threading.Tasks;
using DemoApp.Domain;
using DemoApp.Infrastructure.Persistence;
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

        //see via docker stats. or get the name from container and assert here, but thats ugly (you cant start this test on its own then)
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
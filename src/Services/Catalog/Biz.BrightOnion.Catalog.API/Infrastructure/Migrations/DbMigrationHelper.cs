using FluentMigrator.Runner;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.BrightOnion.Catalog.API.Infrastructure.Migrations
{
    public static class DbMigrationHelper
    {
        public static void EnsureCreated(string connectionString)
        {
            var sqlConnStringBuilder = new SqlConnectionStringBuilder(connectionString);
            var dbName = sqlConnStringBuilder.InitialCatalog;
            sqlConnStringBuilder.InitialCatalog = "master";
            // Only for SQL Server!!!
            using (SqlConnection connection = new SqlConnection(sqlConnStringBuilder.ToString()))
            {
                SqlCommand command = new SqlCommand($"IF (SELECT count(name) FROM sys.databases WHERE name = '{dbName}') = 0 CREATE DATABASE {dbName}", connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}

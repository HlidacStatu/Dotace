using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Npgsql;

namespace DeMinimis
{
    public class DeMinimisRepository
    {
        private string _connectionString;
        
        public DeMinimisRepository(string dbConnectionString)
        {
            _connectionString = dbConnectionString;

        }
        
        private async Task<NpgsqlConnection> OpenConnection()
        {
            var dbConnection = new NpgsqlConnection(_connectionString);
            
            await dbConnection.OpenAsync();
            if (dbConnection.State != ConnectionState.Open)
            {
                await dbConnection.DisposeAsync();
                throw new Exception("Connection is not opened.");
            }

            return dbConnection;
        }
        
        /// <summary>
        /// Inserts many to database
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public async Task InsertMany(DeMinimisDetail?[] results)
        {
            try
            {
                await using var dbConnection = await OpenConnection();

                await dbConnection.InsertAsync(results);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Creates Dotace table in db
        /// </summary>
        /// <returns></returns>
        public async Task CreateTable()
        {
            try
            {
                await using var dbConnection = await OpenConnection();

                await dbConnection.ExecuteAsync(DeMinimisDetail.CreateSqlTableCommand());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
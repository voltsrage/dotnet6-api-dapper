using Dapper.API.Models.AppSettings;
using System.Data;
using System.Data.SqlClient;

namespace Dapper.API.Data.Dapper
{
    public class DapperHandler : IDapperHandler
    {
        private readonly IConfiguration _config;
        private readonly ConnectionStrings _connectionStrings;

        public DapperHandler(
            IConfiguration config, ConnectionStrings connectionStrings)
        {
            _config = config;
            _connectionStrings = connectionStrings;
        }

        // Helper method to get a connection
        private SqlConnection GetConnection(string conString = "MSSQLConnectionLocal")
        {
            return new SqlConnection(_config.GetConnectionString(conString));
        }

        #region Original Methods

        // ===========================================================================
        // Use to execute SP that have no results (Deletes Updates)
        // ===========================================================================
        public async Task<int> ExecuteWithoutReturn(string procedurename, DynamicParameters? param = null, string conString = "MSSQLConnectionLocal", CancellationToken cancellationToken = default)
        {
            using (SqlConnection sqlCon = GetConnection(conString))
            {
                await sqlCon.OpenAsync(cancellationToken);
                return await sqlCon.ExecuteAsync(
                    new CommandDefinition(
                        procedurename,
                        param,
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken));
            }
        }

        // ===========================================================================
        // Use to execute SQL statements that have no results (Deletes Updates)
        // ===========================================================================
        public async Task<int> ExecuteWithoutReturnSql(string sql, DynamicParameters? param = null, string conString = "MSSQLConnectionLocal", CancellationToken cancellationToken = default)
        {
            using (SqlConnection sqlCon = GetConnection(conString))
            {
                await sqlCon.OpenAsync(cancellationToken);
                return await sqlCon.ExecuteAsync(
                    new CommandDefinition(
                        sql,
                        param,
                        commandType: CommandType.Text,
                        cancellationToken: cancellationToken));
            }
        }

        // ===========================================================================
        // Use to execute SP that return a single scalar value
        // ===========================================================================
        public async Task<T> ExecuteWithScalar<T>(string procedurename, DynamicParameters? param = null, string conString = "MSSQLConnectionLocal", CancellationToken cancellationToken = default)
        {
            using (SqlConnection sqlCon = GetConnection(conString))
            {
                await sqlCon.OpenAsync(cancellationToken);
                var ans = await sqlCon.ExecuteScalarAsync(
                    new CommandDefinition(
                        procedurename,
                        param,
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken));

                return (T)Convert.ChangeType(ans, typeof(T));
            }
        }

        // ===========================================================================
        // Use to execute SQL statements that return a single scalar value
        // ===========================================================================
        public async Task<T> ExecuteWithScalarSQL<T>(string sql, DynamicParameters? param = null, string conString = "MSSQLConnectionLocal", CancellationToken cancellationToken = default)
        {
            using (SqlConnection sqlCon = GetConnection(conString))
            {
                await sqlCon.OpenAsync(cancellationToken);
                var ans = await sqlCon.ExecuteScalarAsync(
                    new CommandDefinition(
                        sql,
                        param,
                        commandType: CommandType.Text,
                        cancellationToken: cancellationToken));

                return (T)Convert.ChangeType(ans, typeof(T));
            }
        }

        // ===========================================================================
        // Use to query a list of results using an SP, async version
        // ===========================================================================
        public async Task<IEnumerable<T>> ReturnList<T>(string procedurename, DynamicParameters? param = null, string conString = "MSSQLConnectionLocal", CancellationToken cancellationToken = default)
        {
            using (SqlConnection sqlCon = GetConnection(conString))
            {
                await sqlCon.OpenAsync(cancellationToken);
                return await sqlCon.QueryAsync<T>(
                    new CommandDefinition(
                        procedurename,
                        param,
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken));
            }
        }

        // ===========================================================================
        // Use to query a list of results using an SP, sync version
        // ===========================================================================
        public IEnumerable<T> ReturnListSync<T>(string procedurename, DynamicParameters? param = null, string conString = "MSSQLConnectionLocal")
        {
            using (SqlConnection sqlCon = GetConnection(conString))
            {
                sqlCon.Open();
                return sqlCon.Query<T>(procedurename, param, commandType: CommandType.StoredProcedure);
            }
        }

        // ===========================================================================
        // Use to query a single row of results using an SP, async version
        // ===========================================================================
        public async Task<T> ReturnRow<T>(string procedurename, DynamicParameters? param = null, string conString = "MSSQLConnectionLocal", CancellationToken cancellationToken = default)
        {
            using (SqlConnection sqlCon = GetConnection(conString))
            {
                await sqlCon.OpenAsync(cancellationToken);
                return await sqlCon.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        procedurename,
                        param,
                        commandType: CommandType.StoredProcedure,
                        cancellationToken: cancellationToken));
            }
        }

        // ===========================================================================
        // Use to query a single row of results using an SQL statement, async version
        // ===========================================================================
        public async Task<T> ReturnRowSql<T>(string sql, DynamicParameters? param = null, string conString = "MSSQLConnectionLocal", CancellationToken cancellationToken = default)
        {
            using (SqlConnection sqlCon = GetConnection(conString))
            {
                await sqlCon.OpenAsync(cancellationToken);
                return await sqlCon.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        sql,
                        param,
                        commandType: CommandType.Text,
                        cancellationToken: cancellationToken));
            }
        }

        // ===========================================================================
        // Use to query a list of results using an SQL statement, async version
        // ===========================================================================
        public async Task<IEnumerable<T>> ReturnListSql<T>(string sql, DynamicParameters? param = null, string conString = "MSSQLConnectionLocal", CancellationToken cancellationToken = default)
        {
            using (SqlConnection sqlCon = GetConnection(conString))
            {
                await sqlCon.OpenAsync(cancellationToken);
                return await sqlCon.QueryAsync<T>(
                    new CommandDefinition(
                        sql,
                        param,
                        commandType: CommandType.Text,
                        cancellationToken: cancellationToken));
            }
        }

        #endregion

        #region Transaction Methods

        // ===========================================================================
        // Execute multiple operations within a transaction
        // ===========================================================================
        public async Task<bool> ExecuteInTransaction(Func<IDbConnection, IDbTransaction, CancellationToken, Task> transactionBody, string conString = "MSSQLConnectionLocal", CancellationToken cancellationToken = default)
        {
            using (SqlConnection sqlCon = GetConnection(conString))
            {
                await sqlCon.OpenAsync(cancellationToken);
                using (var transaction = sqlCon.BeginTransaction())
                {
                    try
                    {
                        await transactionBody(sqlCon, transaction, cancellationToken);

                        // Check if operation was cancelled before committing
                        cancellationToken.ThrowIfCancellationRequested();

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // ===========================================================================
        // Execute stored procedure within a transaction
        // ===========================================================================
        public async Task<int> ExecuteWithoutReturnInTransaction(string procedurename, IDbTransaction transaction, DynamicParameters? param = null, CancellationToken cancellationToken = default)
        {
            return await transaction.Connection.ExecuteAsync(
                new CommandDefinition(
                    procedurename,
                    param,
                    transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken));
        }

        // ===========================================================================
        // Execute SQL within a transaction
        // ===========================================================================
        public async Task<int> ExecuteWithoutReturnSqlInTransaction(string sql, IDbTransaction transaction, DynamicParameters? param = null, CancellationToken cancellationToken = default)
        {
            return await transaction.Connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    param,
                    transaction,
                    commandType: CommandType.Text,
                    cancellationToken: cancellationToken));
        }

        // ===========================================================================
        // Execute scalar SP within a transaction
        // ===========================================================================
        public async Task<T> ExecuteWithScalarInTransaction<T>(string procedurename, IDbTransaction transaction, DynamicParameters? param = null, CancellationToken cancellationToken = default)
        {
            var ans = await transaction.Connection.ExecuteScalarAsync(
                new CommandDefinition(
                    procedurename,
                    param,
                    transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken));

            return (T)Convert.ChangeType(ans, typeof(T));
        }

        // ===========================================================================
        // Execute scalar SQL within a transaction
        // ===========================================================================
        public async Task<T> ExecuteWithScalarSqlInTransaction<T>(string sql, IDbTransaction transaction, DynamicParameters? param = null, CancellationToken cancellationToken = default)
        {
            var ans = await transaction.Connection.ExecuteScalarAsync(
                new CommandDefinition(
                    sql,
                    param,
                    transaction,
                    commandType: CommandType.Text,
                    cancellationToken: cancellationToken));

            return (T)Convert.ChangeType(ans, typeof(T));
        }

        // ===========================================================================
        // Query a list with SP within a transaction
        // ===========================================================================
        public async Task<IEnumerable<T>> ReturnListInTransaction<T>(string procedurename, IDbTransaction transaction, DynamicParameters? param = null, CancellationToken cancellationToken = default)
        {
            return await transaction.Connection.QueryAsync<T>(
                new CommandDefinition(
                    procedurename,
                    param,
                    transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken));
        }

        // ===========================================================================
        // Query a list with SQL within a transaction
        // ===========================================================================
        public async Task<IEnumerable<T>> ReturnListSqlInTransaction<T>(string sql, IDbTransaction transaction, DynamicParameters? param = null, CancellationToken cancellationToken = default)
        {
            return await transaction.Connection.QueryAsync<T>(
                new CommandDefinition(
                    sql,
                    param,
                    transaction,
                    commandType: CommandType.Text,
                    cancellationToken: cancellationToken));
        }

        // ===========================================================================
        // Query a single row with SP within a transaction
        // ===========================================================================
        public async Task<T> ReturnRowInTransaction<T>(string procedurename, IDbTransaction transaction, DynamicParameters? param = null, CancellationToken cancellationToken = default)
        {
            return await transaction.Connection.QueryFirstOrDefaultAsync<T>(
                new CommandDefinition(
                    procedurename,
                    param,
                    transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken));
        }

        // ===========================================================================
        // Query a single row with SQL within a transaction
        // ===========================================================================
        public async Task<T> ReturnRowSqlInTransaction<T>(string sql, IDbTransaction transaction, DynamicParameters? param = null, CancellationToken cancellationToken = default)
        {
            return await transaction.Connection.QueryFirstOrDefaultAsync<T>(
                new CommandDefinition(
                    sql,
                    param,
                    transaction,
                    commandType: CommandType.Text,
                    cancellationToken: cancellationToken));
        }

        #endregion
    }
}

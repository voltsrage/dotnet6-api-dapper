using System.Data;

namespace Dapper.API.Data.Dapper
{
    public interface IDapperHandler
    {

        Task<bool> ExecuteInTransaction(Func<IDbConnection, IDbTransaction, CancellationToken, Task> transactionBody, string conString = "MSSQLConnectionLocal", CancellationToken cancellationToken = default);

        Task<int> ExecuteWithoutReturn(string procedurename, DynamicParameters? param = null, string conString = "MSSQLConnectionLocal", CancellationToken cancellationToken = default);

        Task<int> ExecuteWithoutReturnInTransaction(string procedurename, IDbTransaction transaction, DynamicParameters? param = null, CancellationToken cancellationToken = default);

        Task<int> ExecuteWithoutReturnSql(string sql, DynamicParameters? param = null, string conString = "MSSQLConnectionLocal", CancellationToken cancellationToken = default);

        Task<int> ExecuteWithoutReturnSqlInTransaction(string sql, IDbTransaction transaction, DynamicParameters? param = null, CancellationToken cancellationToken = default);

        Task<T> ExecuteWithScalar<T>(string procedurename, DynamicParameters? param = null, string conString = "MSSQLConnectionLocal", CancellationToken cancellationToken = default);

        Task<T> ExecuteWithScalarInTransaction<T>(string procedurename, IDbTransaction transaction, DynamicParameters? param = null, CancellationToken cancellationToken = default);

        Task<T> ExecuteWithScalarSQL<T>(string sql, DynamicParameters? param = null, string conString = "MSSQLConnectionLocal", CancellationToken cancellationToken = default);

        Task<T> ExecuteWithScalarSqlInTransaction<T>(string sql, IDbTransaction transaction, DynamicParameters? param = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> ReturnList<T>(string procedurename, DynamicParameters? param = null, string conString = "MSSQLConnectionLocal", CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> ReturnListInTransaction<T>(string procedurename, IDbTransaction transaction, DynamicParameters? param = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> ReturnListSql<T>(string sql, DynamicParameters? param = null, string conString = "MSSQLConnectionLocal", CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> ReturnListSqlInTransaction<T>(string sql, IDbTransaction transaction, DynamicParameters? param = null, CancellationToken cancellationToken = default);

        IEnumerable<T> ReturnListSync<T>(string procedurename, DynamicParameters? param = null, string conString = "MSSQLConnectionLocal");

        Task<T> ReturnRow<T>(string procedurename, DynamicParameters? param = null, string conString = "MSSQLConnectionLocal", CancellationToken cancellationToken = default);

        Task<T> ReturnRowInTransaction<T>(string procedurename, IDbTransaction transaction, DynamicParameters? param = null, CancellationToken cancellationToken = default);

        Task<T> ReturnRowSql<T>(string sql, DynamicParameters? param = null, string conString = "MSSQLConnectionLocal", CancellationToken cancellationToken = default);

        Task<T> ReturnRowSqlInTransaction<T>(string sql, IDbTransaction transaction, DynamicParameters? param = null, CancellationToken cancellationToken = default);
    }
}
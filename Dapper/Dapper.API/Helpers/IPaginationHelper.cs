using Dapper.API.Models.Pagination;

namespace Dapper.API.Helpers
{
    /// <summary>
    /// Interface for pagination helper
    /// </summary>
    public interface IPaginationHelper
    {
        /// <summary>
        /// Gets paginated data with custom joins
        /// </summary>
        /// <typeparam name="T">Type of entity to retrieve</typeparam>
        /// <param name="request">Pagination request</param>
        /// <param name="queryBuilder">Pre-configured query builder with custom joins</param>
        /// <param name="mainTableName">Name of the main table</param>
        /// <param name="filterableColumns">Dictionary mapping filter keys to column names</param>
        /// <param name="searchableColumns">Columns to include in search</param>
        /// <returns>Paginated result</returns>
        Task<PaginatedResult<T>> GetPaginatedDataWithJoinsAsync<T>(
            PaginationRequest request,
            QueryBuilder queryBuilder,
            string mainTableName,
            Dictionary<string, string> filterableColumns = null,
            string[] searchableColumns = null);


        /// <summary>
        /// Gets paginated data using a base query with optional search and filters
        /// </summary>
        /// <typeparam name="T">Type of entity to retrieve</typeparam>
        /// <param name="request">Pagination request</param>
        /// <param name="tableName">Name of the database table</param>
        /// <param name="columns">Columns to select</param>
        /// <param name="searchableColumns">Columns to include in search</param>
        /// <param name="filterableColumns">Dictionary mapping filter keys to column names</param>
        /// <param name="baseCondition">Base WHERE condition</param>
        /// <param name="sortColumn">Default sort column</param>
        /// <param name="sortDirection">Default sort direction</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated result</returns>
        Task<PaginatedResult<T>> GetPaginatedResultAsync<T>(
            PaginationRequest request,
            string tableName,
            string columns = "*",
            string[] searchableColumns = null,
            Dictionary<string, string> filterableColumns = null,
            string baseCondition = null,
            string sortColumn = null,
            string sortDirection = "ASC",
            CancellationToken cancellationToken = default);
    }
}
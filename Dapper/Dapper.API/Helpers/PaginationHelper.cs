using Dapper.API.Data.Dapper;
using Dapper.API.Dtos.Hotels;
using Dapper.API.Models.Pagination;
using System.Text;
using System.Threading;

namespace Dapper.API.Helpers
{
    public class PaginationHelper
    {
        private readonly IDapperHandler _dataAccess;

        /// <summary>
        /// Creates a new pagination helper
        /// </summary>
        /// <param name="dapperHandler"></param>
        public PaginationHelper(IDapperHandler dapperHandler)
        {
            _dataAccess = dapperHandler;
        }

        public async Task<PaginatedResult<T>> GetPaginatedResultAsync<T>(
            PaginationRequest request, 
            string tableName,
            string columns = "*",
            string[] searchableColumns = null, 
            string baseCondition = null,
            string sortColumn = null,
            string sortDirection = "ASC",
            CancellationToken cancellationToken = default)
        {
            // Validate inputs
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("Table name is required", nameof(tableName));

            // Setup parameters
            var parameters = new DynamicParameters();
            parameters.Add("@Offset", request.Skip);
            parameters.Add("@PageSize", request.PageSize);

            // Build the query
            var sql = new StringBuilder();

            // Build the base condition
            string whereClause = string.IsNullOrEmpty(baseCondition) ? "1=1" : baseCondition;

            // Build the search condition
            if (request.HasSearch && searchableColumns != null && searchableColumns.Length > 0)
            {
                parameters.Add("@SearchTerm", $"%{request.SearchTerm.ToLower()}%");

                var searchConditions = searchableColumns
                    .Select(column => $"LOWER({column}) LIKE @SearchTerm")
                    .ToList();

                whereClause += $" AND ({string.Join(" OR ", searchConditions)})";
            }

            // Determine sort column and direction
            string effectiveSortColumn = request.HasSorting ? request.SortColumn : (sortColumn ?? "1");
            string effectiveSortDirection = request.HasSorting
                ? (request.IsAscending ? "ASC" : "DESC")
                : sortDirection;

            // Construct the main query
            sql.AppendLine($"SELECT {columns}");
            sql.AppendLine($"FROM {tableName}");
            sql.AppendLine($"WHERE {whereClause}");
            sql.AppendLine($"ORDER BY {effectiveSortColumn} {effectiveSortDirection} ");
            sql.AppendLine("OFFSET @Offset ROWS");
            sql.AppendLine("FETCH NEXT @PageSize ROWS ONLY;");

            // Add the count query
            sql.AppendLine($"SELECT COUNT(1) FROM {tableName}");
            sql.AppendLine($"WHERE {whereClause};");

            using (var multiQuery = await _dataAccess.QueryMultipleAsync(
            sql.ToString(),
              parameters,
              cancellationToken: cancellationToken))
            {
                // Read both result sets
                var hotels = (await multiQuery.ReadAsync<T>()).ToList();
                var totalCount = await multiQuery.ReadFirstOrDefaultAsync<int>();

                // Create paginated result with metadata
                return new PaginatedResult<T>(
                    items: hotels,
                    totalCount: totalCount,
                    page: request.Page,
                    pageSize: request.PageSize);
            }
        }
    }
}

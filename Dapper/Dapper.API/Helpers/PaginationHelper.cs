using Dapper.API.Data.Dapper;
using Dapper.API.Dtos.Hotels;
using Dapper.API.Exceptions;
using Dapper.API.Models.Pagination;
using System.Data.SqlClient;
using System.Text;
using System.Threading;

namespace Dapper.API.Helpers
{
    public class PaginationHelper
    {
        private readonly IDapperHandler _dataAccess;
        private readonly ILogger<PaginationHelper> _logger;
        private const string COMPONENT_NAME = "PaginationHelper";

        /// <summary>
        /// Creates a new pagination helper
        /// </summary>
        /// <param name="dapperHandler"></param>
        public PaginationHelper(
            IDapperHandler dapperHandler,
            ILogger<PaginationHelper> logger)
        {
            _dataAccess = dapperHandler;
            _logger = logger;
        }

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
        /// <returns>Paginated result</returns>
        public async Task<PaginatedResult<T>> GetPaginatedResultAsync<T>(
            PaginationRequest request, 
            string tableName,
            string columns = "*",
            string[] searchableColumns = null, 
            Dictionary<string, string> filterableColumns = null,
            string baseCondition = null,
            string sortColumn = null,
            string sortDirection = "ASC",
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate inputs
                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                // Validate inputs
                if (string.IsNullOrEmpty(tableName))
                    throw new ArgumentException("Table name is required", nameof(tableName));

                // Build query components
                var queryBuilder = new QueryBuilder(tableName)
                    .Select(columns)
                    .Where(baseCondition);

                // Apply search if needed
                if (request.HasSearch && searchableColumns != null && searchableColumns.Length > 0)
                {
                    try
                    {
                        queryBuilder.WithSearch(request.SearchTerm, searchableColumns);
                    }
                    catch (Exception ex)
                    {
                        throw new SearchException(
                            "Error applying search criteria",
                            COMPONENT_NAME,
                            nameof(GetPaginatedResultAsync),
                            tableName,
                            request.SearchTerm,
                            searchableColumns,
                            ex);
                    }
                }

                // Apply filters if provided
                if (request.HasFilters && filterableColumns != null)
                {
                    ApplyFilters(queryBuilder, request.Filters, filterableColumns,tableName);
                }

                // Apply sorting
                //string effectiveSortColumn = request.HasSorting ? request.SortColumn : (sortColumn ?? "1");
                //string effectiveSortDirection = request.HasSorting
                //    ? (request.IsAscending ? "ASC" : "DESC")
                //    : sortDirection;
                //queryBuilder.OrderBy(effectiveSortColumn, effectiveSortDirection);

                try
                {
                    if (request.HasSorting)
                    {
                        if (request.IsAscending)
                        {
                            queryBuilder.OrderBy(request.SortColumn);
                        }
                        else
                        {
                            queryBuilder.OrderByDescending(request.SortColumn);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new SortException(
                        "Error applying sort criteria",
                        COMPONENT_NAME,
                        nameof(GetPaginatedResultAsync),
                        tableName,
                        request.HasSorting ? request.SortColumn : sortColumn ?? "1",
                        request.HasSorting ? (request.IsAscending ? "ASC" : "DESC") : sortDirection,
                        ex);
                }


                // Execute the paginated query
                return await ExecutePaginatedQueryAsync<T>(
                    queryBuilder,
                    request.Page,
                    request.Skip,
                    request.PageSize,
                    tableName);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid pagination parameters: {Message}", ex.Message);
                throw new InvalidPaginationParametersException(
                    ex.Message,
                    COMPONENT_NAME,
                    nameof(GetPaginatedResultAsync),
                    tableName,
                    ex.ParamName ?? "unknown",
                    ex);
            }
            catch (FilterException ex)
            {
                _logger.LogError(ex, "Filter error: {Message}, Key: {Key}, Value: {Value}",
                    ex.Message, ex.FilterKey, ex.FilterValue);
                throw; // Re-throw as it's already our custom exception
            }
            catch (SearchException ex)
            {
                _logger.LogError(ex, "Search error: {Message}, Term: {Term}",
                    ex.Message, ex.SearchTerm);
                throw; // Re-throw as it's already our custom exception
            }
            catch (SortException ex)
            {
                _logger.LogError(ex, "Sort error: {Message}, Column: {Column}",
                    ex.Message, ex.SortColumn);
                throw; // Re-throw as it's already our custom exception
            }
            catch (PaginationDatabaseException ex)
            {
                _logger.LogError(ex, "Database error in pagination: {Message}", ex.Message);
                throw; // Re-throw as it's already our custom exception
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error in pagination: {Message}", ex.Message);
                throw new PaginationDatabaseException(
                    "A database error occurred while retrieving paged data",
                    COMPONENT_NAME,
                    nameof(GetPaginatedResultAsync),
                    tableName,
                    "SQL error", // Don't include actual SQL for security
                    ex);
            }
            catch (Exception ex) when (
                ex is not PaginationException &&
                ex is not FilterException &&
                ex is not SearchException &&
                ex is not SortException)
            {
                _logger.LogError(ex, "Unexpected error in pagination: {Message}", ex.Message);
                throw new PaginationException(
                    "An unexpected error occurred during pagination",
                    COMPONENT_NAME,
                    nameof(GetPaginatedResultAsync),
                    tableName,
                    ex);
            }

        }

        /// <summary>
        /// Gets paginated data with custom joins
        /// </summary>
        /// <typeparam name="T">Type of entity to retrieve</typeparam>
        /// <param name="request">Pagination request</param>
        /// <param name="queryBuilder">Pre-configured query builder with custom joins</param>
        /// <returns>Paginated result</returns>
        public async Task<PaginatedResult<T>> GetPaginatedDataWithJoinsAsync<T>(
            PaginationRequest request,
            QueryBuilder queryBuilder,
            string mainTableName,
            Dictionary<string, string> filterableColumns = null
            )
        {
            try
            {
                // Validate inputs
                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (queryBuilder == null)
                    throw new ArgumentNullException(nameof(queryBuilder));

                // Apply filters if provided
                if (request.HasFilters && filterableColumns != null)
                {
                    ApplyFilters(queryBuilder, request.Filters, filterableColumns,mainTableName);
                }


                // Apply sorting if not already specified
                if (request.HasSorting && !queryBuilder.HasOrdering)
                {
                    try
                    {
                        if (request.IsAscending)
                        {
                            queryBuilder.OrderBy(request.SortColumn);
                        }
                        else
                        {
                            queryBuilder.OrderByDescending(request.SortColumn);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new SortException(
                            "Error applying sort criteria",
                            COMPONENT_NAME,
                            nameof(GetPaginatedDataWithJoinsAsync),
                            mainTableName,
                            request.SortColumn,
                            request.IsAscending ? "ASC" : "DESC",
                            ex);
                    }

                    //queryBuilder.OrderBy(
                    //    request.SortColumn,
                    //    request.IsAscending ? "ASC" : "DESC");
                }

                // Execute the paginated query
                return await ExecutePaginatedQueryAsync<T>(
                    queryBuilder,
                    request.Page,
                    request.Skip,
                    request.PageSize, mainTableName);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid pagination parameters for join query: {Message}", ex.Message);
                throw new InvalidPaginationParametersException(
                    ex.Message,
                    COMPONENT_NAME,
                    nameof(GetPaginatedDataWithJoinsAsync),
                    mainTableName,
                    ex.ParamName ?? "unknown",
                    ex);
            }
            catch (FilterException ex)
            {
                _logger.LogError(ex, "Filter error in join query: {Message}, Key: {Key}, Value: {Value}",
                    ex.Message, ex.FilterKey, ex.FilterValue);
                throw; // Re-throw as it's already our custom exception
            }
            catch (SortException ex)
            {
                _logger.LogError(ex, "Sort error in join query: {Message}, Column: {Column}",
                    ex.Message, ex.SortColumn);
                throw; // Re-throw as it's already our custom exception
            }
            catch (PaginationDatabaseException ex)
            {
                _logger.LogError(ex, "Database error in join pagination: {Message}", ex.Message);
                throw; // Re-throw as it's already our custom exception
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error in join pagination: {Message}", ex.Message);
                throw new PaginationDatabaseException(
                    "A database error occurred while retrieving paged data with joins",
                    COMPONENT_NAME,
                    nameof(GetPaginatedDataWithJoinsAsync),
                    mainTableName,
                    "SQL error", // Don't include actual SQL for security
                    ex);
            }
            catch (Exception ex) when (
                ex is not PaginationException &&
                ex is not FilterException &&
                ex is not SortException)
            {
                _logger.LogError(ex, "Unexpected error in join pagination: {Message}", ex.Message);
                throw new PaginationException(
                    "An unexpected error occurred during pagination with joins",
                    COMPONENT_NAME,
                    nameof(GetPaginatedDataWithJoinsAsync),
                    mainTableName,
                    ex);
            }
        }

        /// <summary>
        /// Applies filters to the query builder
        /// </summary>
        private void ApplyFilters(
            QueryBuilder queryBuilder,
            Dictionary<string, string> filters,
            Dictionary<string, string> filterableColumns,
            string tableName)
        {
            foreach (var filter in filters)
            {
                try
                {
                    // Extract the base filter key (without operator)
                    string filterKey = filter.Key;
                    string op = "eq";

                    if (filterKey.Contains("__"))
                    {
                        var parts = filterKey.Split("__", 2);
                        filterKey = parts[0];
                        op = parts[1].ToLowerInvariant();
                    }

                    // Verify that this is a filterable column
                    if (!filterableColumns.TryGetValue(filterKey, out string columnName))
                    {
                        _logger.LogWarning("Ignoring filter on non-filterable column: {Column}", filterKey);
                        continue;
                    }

                    // Create unique parameter name to prevent conflicts
                    string paramName = $"@Filter_{Guid.NewGuid().ToString("N").Substring(0, 8)}";

                    // Apply the filter based on operator
                    ApplyFilterWithOperator(queryBuilder, columnName, op, filter.Value, paramName, filterKey, tableName);
                }
                catch (FilterException)
                {
                    throw; // Re-throw existing filter exceptions
                }
                catch (Exception ex)
                {
                    // Extract operation if present
                    string filterOp = "eq";
                    string actualFilterKey = filter.Key;

                    if (filter.Key.Contains("__"))
                    {
                        var parts = filter.Key.Split("__", 2);
                        actualFilterKey = parts[0];
                        filterOp = parts[1].ToLowerInvariant();
                    }

                    throw new FilterException(
                        $"Error applying filter: {ex.Message}",
                        COMPONENT_NAME,
                        nameof(ApplyFilters),
                        tableName,
                        actualFilterKey,
                        filter.Value,
                        filterOp,
                        ex);
                }

            }
        }

        /// <summary>
        /// Applies a filter with the specified operator
        /// </summary>
        private void ApplyFilterWithOperator(
            QueryBuilder queryBuilder,
            string columnName,
            string op,
            string value,
            string paramName,
            string filterKey,
            string tableName)
        {
            try
            {
                switch (op)
                {
                    case "eq": // equals
                        queryBuilder.AddParameter(paramName, value);
                        queryBuilder.Where($"{columnName} = {paramName}");
                        break;

                    case "neq": // not equals
                        queryBuilder.AddParameter(paramName, value);
                        queryBuilder.Where($"{columnName} <> {paramName}");
                        break;

                    case "gt": // greater than
                        queryBuilder.AddParameter(paramName, value);
                        queryBuilder.Where($"{columnName} > {paramName}");
                        break;

                    case "gte": // greater than or equal
                        queryBuilder.AddParameter(paramName, value);
                        queryBuilder.Where($"{columnName} >= {paramName}");
                        break;

                    case "lt": // less than
                        queryBuilder.AddParameter(paramName, value);
                        queryBuilder.Where($"{columnName} < {paramName}");
                        break;

                    case "lte": // less than or equal
                        queryBuilder.AddParameter(paramName, value);
                        queryBuilder.Where($"{columnName} <= {paramName}");
                        break;

                    case "contains": // contains substring
                        queryBuilder.AddParameter(paramName, $"%{value}%");
                        queryBuilder.Where($"{columnName} LIKE {paramName}");
                        break;

                    case "startswith": // starts with
                        queryBuilder.AddParameter(paramName, $"{value}%");
                        queryBuilder.Where($"{columnName} LIKE {paramName}");
                        break;

                    case "endswith": // ends with
                        queryBuilder.AddParameter(paramName, $"%{value}");
                        queryBuilder.Where($"{columnName} LIKE {paramName}");
                        break;

                    case "in": // in a list of values
                        var values = value.Split(',').Select(v => v.Trim()).ToList();
                        if (values.Count > 0)
                        {
                            queryBuilder.Where($"{columnName} IN ({string.Join(",", values.Select((_, i) => $"{paramName}_{i}"))})");
                            for (int i = 0; i < values.Count; i++)
                            {
                                queryBuilder.AddParameter($"{paramName}_{i}", values[i]);
                            }
                        }
                        else
                        {
                            throw new ArgumentException("In operator requires at least one value");
                        }
                        break;

                    case "notin": // not in a list of values
                        var excludedValues = value.Split(',').Select(v => v.Trim()).ToList();
                        if (excludedValues.Count > 0)
                        {
                            queryBuilder.Where($"{columnName} NOT IN ({string.Join(",", excludedValues.Select((_, i) => $"{paramName}_{i}"))})");
                            for (int i = 0; i < excludedValues.Count; i++)
                            {
                                queryBuilder.AddParameter($"{paramName}_{i}", excludedValues[i]);
                            }
                        }
                        else
                        {
                            throw new ArgumentException("NotIn operator requires at least one value");
                        }
                        break;

                    case "isnull": // is null
                        queryBuilder.Where($"{columnName} IS NULL");
                        break;

                    case "isnotnull": // is not null
                        queryBuilder.Where($"{columnName} IS NOT NULL");
                        break;

                    case "between": // between two values
                        var rangeParts = value.Split(',');
                        if (rangeParts.Length == 2)
                        {
                            queryBuilder.AddParameter($"{paramName}_from", rangeParts[0].Trim());
                            queryBuilder.AddParameter($"{paramName}_to", rangeParts[1].Trim());
                            queryBuilder.Where($"{columnName} BETWEEN {paramName}_from AND {paramName}_to");
                        }
                        else
                        {
                            throw new ArgumentException("Between operator requires two comma-separated values");
                        }
                        break;

                    default:
                        throw new ArgumentException($"Unsupported filter operator: {op}");
                }
            }
            catch (Exception ex)
            {
                throw new FilterException(
                    $"Error applying filter with operator '{op}': {ex.Message}",
                    COMPONENT_NAME,
                    nameof(ApplyFilterWithOperator),
                    tableName,
                    filterKey,
                    value,
                    op,
                    ex);
            }
        }

        /// <summary>
        /// Executes a paginated query using the provided query builder
        /// </summary>
        private async Task<PaginatedResult<T>> ExecutePaginatedQueryAsync<T>(
            QueryBuilder queryBuilder,
            int page,
            int skip,
            int pageSize, 
            string tableName,
            CancellationToken cancellationToken = default)
        {
            // Add pagination parameters
            queryBuilder.Parameters.Add("@Offset", skip);
            queryBuilder.Parameters.Add("@PageSize", pageSize);

            // Build the complete SQL query
            var sql = new StringBuilder();

            // Data query with pagination
            sql.Append(queryBuilder.BuildSelectQuery());


            sql.AppendLine(" OFFSET @Offset ROWS ");
            sql.AppendLine(" FETCH NEXT @PageSize ROWS ONLY; ");

            // Count query
            sql.AppendLine(queryBuilder.BuildCountQuery());

            string sqlQuery = sql.ToString();

            try
            {
                using (var multiQuery = await _dataAccess.QueryMultipleAsync(
                 sql.ToString(),
                 queryBuilder.Parameters,
                 cancellationToken: cancellationToken))
                {
                    // Read both result sets
                    var hotels = (await multiQuery.ReadAsync<T>()).ToList();
                    var totalCount = await multiQuery.ReadFirstOrDefaultAsync<int>();

                    // Create paginated result with metadata
                    return new PaginatedResult<T>(
                        items: hotels,
                        totalCount: totalCount,
                        page: page,
                        pageSize: pageSize);
                }
            }
            catch (SqlException ex)
            {
                // Log the detailed SQL query for troubleshooting but don't expose it in the exception
                _logger.LogError(ex, "SQL Error executing query: {Query}", sqlQuery);

                // In development, you might want to include the SQL, but in production it's a security risk
                #if DEBUG
                throw new PaginationDatabaseException(
                    "Database error during pagination",
                    COMPONENT_NAME,
                    nameof(ExecutePaginatedQueryAsync),
                    tableName,
                    sqlQuery,
                    ex);
                #else
                throw new PaginationDatabaseException(
                    "Database error during pagination",
                    COMPONENT_NAME,
                    nameof(ExecutePaginatedQueryAsync),
                    tableName,
                    "SQL error", // Don't include actual SQL for security
                    ex);
                #endif
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing paginated query");
                throw new PaginationException(
                    "Failed to execute paginated query",
                    COMPONENT_NAME,
                    nameof(ExecutePaginatedQueryAsync),
                    tableName,
                    ex);
            }


        }
    }
}

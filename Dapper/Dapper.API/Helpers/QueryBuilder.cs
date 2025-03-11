using Dapper.API.Enums.StandardEnums;
using Dapper.API.Models.Pagination;
using System.Text;

namespace Dapper.API.Helpers
{
    /// <summary>
    /// Builder for constructing SQL queries
    /// </summary>
    public class QueryBuilder
    {
        // <summary>
        /// Parameters for the query
        /// </summary>
        public DynamicParameters Parameters { get; } = new DynamicParameters();

        private readonly string _baseTable;
        private readonly string _baseAlias;
        private readonly List<string> _selectColumns = new List<string>();
        private readonly List<string> _whereConditions = new List<string>();
        private readonly List<string> _orderByColumns = new List<string>();
        private readonly List<JoinClause> _joins = new List<JoinClause>();
        private string _orderBy = null;
        private bool _isDescending = false;

        /// <summary>
        /// Indicates if ordering has been specified
        /// </summary>
        public bool HasOrdering => !string.IsNullOrEmpty(_orderBy);

        /// <summary>
        /// Creates a new query builder
        /// </summary>
        /// <param name="baseTable">Base table name</param>
        /// <param name="baseAlias">Optional alias for base table</param>
        public QueryBuilder(string baseTable, string baseAlias = null)
        {
            _baseTable = baseTable ?? throw new ArgumentNullException(nameof(baseTable));
            _baseAlias = string.IsNullOrEmpty(baseAlias) ? baseTable : baseAlias;
        }

        /// <summary>
        /// Specifies columns to select
        /// </summary>
        /// <param name="columns">Columns or expressions to select</param>
        /// <returns>The builder for chaining</returns>
        public QueryBuilder Select(string columns)
        {
            if (!string.IsNullOrEmpty(columns))
            {
                _selectColumns.Add(columns);
            }
            return this;
        }

        /// <summary>
        /// Adds a JOIN clause to the query
        /// </summary>
        /// <param name="joinType">Type of join</param>
        /// <param name="table">Table to join</param>
        /// <param name="alias">Alias for the joined table</param>
        /// <param name="on">Join condition</param>
        /// <returns>The builder for method chaining</returns>
        public QueryBuilder Join(JoinType joinType, string table, string alias, string on)
        {
            _joins.Add(new JoinClause(joinType, table, alias, on));
            return this;
        }

        /// <summary>
        /// Adds an INNER JOIN to the query
        /// </summary>
        /// <param name="table">Table to join</param>
        /// <param name="alias">Alias for the joined table</param>
        /// <param name="on">Join condition</param>
        /// <returns>The builder for chaining</returns>
        public QueryBuilder InnerJoin(string table, string alias, string on) =>
            Join(JoinType.Inner, table, alias, on);

        /// <summary>
        /// Adds a LEFT JOIN to the query
        /// </summary>
        /// <param name="table">Table to join</param>
        /// <param name="alias">Alias for the joined table</param>
        /// <param name="on">Join condition</param>
        /// <returns>The builder for chaining</returns>
        public QueryBuilder LeftJoin(string table, string alias, string on) =>
            Join(JoinType.Left, table, alias, on);

        /// <summary>
        /// Adds a RIGHT JOIN to the query
        /// </summary>
        /// <param name="table">Table to join</param>
        /// <param name="alias">Alias for the joined table</param>
        /// <param name="on">Join condition</param>
        /// <returns>The builder for chaining</returns>
        public QueryBuilder RightJoin(string table, string alias, string on) =>
            Join(JoinType.Right, table, alias, on);

        /// <summary>
        /// Adds a WHERE condition to the query
        /// </summary>
        /// <param name="condition">WHERE condition</param>
        /// <returns>The builder for chaining</returns>
        public QueryBuilder Where(string condition)
        {
            if (!string.IsNullOrEmpty(condition))
            {
                _whereConditions.Add($"({condition})");
            }
            return this;
        }

        /// <summary>
        /// Adds a search condition across specified columns
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="columns">Columns to search</param>
        /// <returns>The builder for chaining</returns>
        public QueryBuilder WithSearch(string searchTerm, params string[] columns)
        {
            if (!string.IsNullOrEmpty(searchTerm) && columns != null && columns.Length > 0)
            {
                // Add parameter for search term
                Parameters.Add("@SearchTerm", $"%{searchTerm.ToLower()}%");

                // Build search conditions
                var searchConditions = columns
                    .Select(column => $"LOWER({column}) LIKE @SearchTerm")
                    .ToList();

                // Add to WHERE conditions
                _whereConditions.Add($"({string.Join(" OR ", searchConditions)})");
            }
            return this;
        }

        ///// <summary>
        ///// Sets the ORDER BY clause
        ///// </summary>
        ///// <param name="column">Column to sort by</param>
        ///// <param name="direction">Sort direction (ASC or DESC)</param>
        ///// <returns>The builder for chaining</returns>
        //public QueryBuilder OrderBy(string column, string direction = "ASC")
        //{
        //    if (!string.IsNullOrEmpty(column))
        //    {
        //        _orderBy = $"{column} {direction}";
        //    }
        //    return this;
        //}

        /// <summary>
        /// Sets the ORDER BY clause
        /// </summary>
        /// <param name="columns">Columns to order by</param>
        /// <returns>The builder for method chaining</returns>
        public QueryBuilder OrderBy(params string[] columns)
        {
            if (columns != null && columns.Length > 0)
            {
                _orderByColumns.Clear();
                _orderByColumns.AddRange(columns);
                _isDescending = false;
            }
            return this;
        }

        /// <summary>
        /// Sets the ORDER BY clause with descending direction
        /// </summary>
        /// <param name="columns">Columns to order by</param>
        /// <returns>The builder for method chaining</returns>
        public QueryBuilder OrderByDescending(params string[] columns)
        {
            if (columns != null && columns.Length > 0)
            {
                _orderByColumns.Clear();
                _orderByColumns.AddRange(columns);
                _isDescending = true;
            }
            return this;
        }

        /// <summary>
        /// Adds a parameter to the query
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <returns>The builder for chaining</returns>
        public QueryBuilder AddParameter(string name, object value)
        {
            Parameters.Add(name, value);
            return this;
        }

        /// <summary>
        /// Builds the SELECT query without pagination
        /// </summary>
        /// <returns>SQL SELECT query</returns>
        public string BuildSelectQuery()
        {
            var sql = new StringBuilder();

            // SELECT clause
            sql.Append("SELECT ");
            if (_selectColumns.Count > 0)
            {
                sql.Append(string.Join(", ", _selectColumns));
            }
            else
            {
                sql.Append($"{_baseAlias}.*");
            }
            sql.AppendLine();

            // FROM clause
            sql.AppendLine($" FROM {_baseTable} AS {_baseAlias} ");

            // JOIN clauses
            foreach (var join in _joins)
            {
                sql.AppendLine($"{join.Type} JOIN {join.Table} AS {join.Alias} ON {join.Condition} ");
            }

            // WHERE clause
            if (_whereConditions.Count > 0)
            {
                sql.AppendLine($"WHERE {string.Join(" AND ", _whereConditions)} ");
            }

            // ORDER BY clause
            //if (!string.IsNullOrEmpty(_orderBy))
            //{
            //    sql.AppendLine($"ORDER BY {_orderBy}");
            //}
            //else
            //{
            //    sql.AppendLine("ORDER BY 1");
            //}
            sql.AppendLine(" ORDER BY ");
            if (_orderByColumns.Count > 0)
            {
                sql.Append(string.Join(", ", _orderByColumns));
                sql.Append(_isDescending ? " DESC " : " ASC ");
            }
            else
            {
                sql.Append("1"); // Default ordering
            }

            return sql.ToString();
        }

        /// <summary>
        /// Builds the COUNT query
        /// </summary>
        /// <returns>SQL COUNT query</returns>
        public string BuildCountQuery()
        {
            var sql = new StringBuilder();

            sql.Append("SELECT COUNT(1) FROM ");
            sql.Append($"{_baseTable} AS {_baseAlias} ");
            sql.AppendLine();

            // JOIN clauses
            foreach (var join in _joins)
            {
                sql.AppendLine($"{join.Type} JOIN {join.Table} AS {join.Alias} ON {join.Condition} ");
            }

            // WHERE clause
            if (_whereConditions.Count > 0)
            {
                sql.AppendLine($"WHERE {string.Join(" AND ", _whereConditions)} ");
            }

            return sql.ToString();
        }
    }
}

using Dapper.API.Enums.StandardEnums;

namespace Dapper.API.Models.Pagination
{
    /// <summary>
    /// Represents a JOIN clause in a SQL query
    /// </summary>
    public class JoinClause
    {
        /// <summary>
        /// JOIN type (INNER JOIN, LEFT JOIN, etc.)
        /// </summary>
        public JoinType Type { get; }

        /// <summary>
        /// Table being joined
        /// </summary>
        public string Table { get; }

        /// <summary>
        /// Alias for the joined table
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// JOIN condition
        /// </summary>
        public string Condition { get; }

        /// <summary>
        /// String representation of the join type
        /// </summary>
        public string JoinTypeString
        {
            get
            {
                return Type switch
                {
                    JoinType.Inner => "INNER JOIN",
                    JoinType.Left => "LEFT JOIN",
                    JoinType.Right => "RIGHT JOIN",
                    _ => "JOIN"
                };
            }
        }

        /// <summary>
        /// Creates a new JOIN clause
        /// </summary>
        public JoinClause(JoinType joinType, string table, string alias, string condition)
        {
            Type = joinType;
            Table = table ?? throw new ArgumentNullException(nameof(table));
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }
    }
}

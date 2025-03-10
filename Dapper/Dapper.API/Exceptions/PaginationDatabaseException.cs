namespace Dapper.API.Exceptions
{
    /// <summary>
    /// Exception thrown when database operations related to pagination fail
    /// </summary>
    public class PaginationDatabaseException : PaginationException
    {
        /// <summary>
        /// SQL statement that caused the exception (only populated in development)
        /// </summary>
        public string SqlStatement { get; }

        /// <summary>
        /// Creates a new pagination database exception
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="component">Component where exception occurred</param>
        /// <param name="function">Function where exception occurred</param>
        /// <param name="table">Table being queried</param>
        /// <param name="sqlStatement">SQL statement that caused the exception</param>
        /// <param name="innerException">Inner exception if available</param>
        /// <param name="additionalData">Additional contextual data</param>
        public PaginationDatabaseException(
            string message,
            string component,
            string function,
            string table,
            string sqlStatement,
            Exception innerException = null,
            Dictionary<string, object> additionalData = null)
            : base(message, component, function, table, innerException, additionalData)
        {
            SqlStatement = sqlStatement;

            // Only add SQL statement to additional data in development
            #if DEBUG
            AdditionalData["sqlStatement"] = sqlStatement;
            #endif
        }
    }
}

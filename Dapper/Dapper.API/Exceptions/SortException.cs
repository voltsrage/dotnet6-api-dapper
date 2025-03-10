namespace Dapper.API.Exceptions
{
    /// <summary>
    /// Exception thrown when sorting operations fail
    /// </summary>
    public class SortException : PaginationException
    {
        /// <summary>
        /// The sort column that caused the exception
        /// </summary>
        public string SortColumn { get; }

        /// <summary>
        /// The sort direction
        /// </summary>
        public string SortDirection { get; }

        /// <summary>
        /// Creates a new sort exception
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="component">Component where exception occurred</param>
        /// <param name="function">Function where exception occurred</param>
        /// <param name="table">Table being queried</param>
        /// <param name="sortColumn">Sort column that caused the exception</param>
        /// <param name="sortDirection">Sort direction</param>
        /// <param name="innerException">Inner exception if available</param>
        /// <param name="additionalData">Additional contextual data</param>
        public SortException(
            string message,
            string component,
            string function,
            string table,
            string sortColumn,
            string sortDirection,
            Exception innerException = null,
            Dictionary<string, object> additionalData = null)
            : base(message, component, function, table, innerException, additionalData)
        {
            SortColumn = sortColumn;
            SortDirection = sortDirection;

            // Add sort details to additional data
            AdditionalData["sortColumn"] = sortColumn;
            AdditionalData["sortDirection"] = sortDirection;
        }
    }
}

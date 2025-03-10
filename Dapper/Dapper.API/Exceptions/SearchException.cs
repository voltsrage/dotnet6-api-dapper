namespace Dapper.API.Exceptions
{
    /// <summary>
    /// Exception thrown when search operations fail
    /// </summary>
    public class SearchException : PaginationException
    {
        /// <summary>
        /// The search term that caused the exception
        /// </summary>
        public string SearchTerm { get; }

        /// <summary>
        /// The columns being searched
        /// </summary>
        public string[] SearchColumns { get; }

        /// <summary>
        /// Creates a new search exception
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="component">Component where exception occurred</param>
        /// <param name="function">Function where exception occurred</param>
        /// <param name="table">Table being queried</param>
        /// <param name="searchTerm">Search term that caused the exception</param>
        /// <param name="searchColumns">Columns being searched</param>
        /// <param name="innerException">Inner exception if available</param>
        /// <param name="additionalData">Additional contextual data</param>
        public SearchException(
            string message,
            string component,
            string function,
            string table,
            string searchTerm,
            string[] searchColumns,
            Exception innerException = null,
            Dictionary<string, object> additionalData = null)
            : base(message, component, function, table, innerException, additionalData)
        {
            SearchTerm = searchTerm;
            SearchColumns = searchColumns;

            // Add search details to additional data
            AdditionalData["searchTerm"] = searchTerm;
            AdditionalData["searchColumns"] = searchColumns;
        }
    }
}

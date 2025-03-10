namespace Dapper.API.Exceptions
{

    /// <summary>
    /// Exception thrown when filtering operations fail
    /// </summary>
    public class FilterException : PaginationException
    {
        /// <summary>
        /// The filter key that caused the exception
        /// </summary>
        public string FilterKey { get; }

        /// <summary>
        /// The filter value that caused the exception
        /// </summary>
        public string FilterValue { get; }

        /// <summary>
        /// The operation being attempted (e.g., 'eq', 'contains', 'in')
        /// </summary>
        public string FilterOperation { get; }

        /// <summary>
        /// Creates a new filter exception
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="component">Component where exception occurred</param>
        /// <param name="function">Function where exception occurred</param>
        /// <param name="table">Table being queried</param>
        /// <param name="filterKey">Filter key that caused the exception</param>
        /// <param name="filterValue">Filter value that caused the exception</param>
        /// <param name="filterOperation">Filter operation that failed</param>
        /// <param name="innerException">Inner exception if available</param>
        /// <param name="additionalData">Additional contextual data</param>
        public FilterException(
            string message,
            string component,
            string function,
            string table,
            string filterKey,
            string filterValue,
            string filterOperation,
            Exception innerException = null,
            Dictionary<string, object> additionalData = null)
            : base(message, component, function, table, innerException, additionalData)
        {
            FilterKey = filterKey;
            FilterValue = filterValue;
            FilterOperation = filterOperation;

            // Add filter details to additional data
            AdditionalData["filterKey"] = filterKey;
            AdditionalData["filterValue"] = filterValue;
            AdditionalData["filterOperation"] = filterOperation;
        }
    }

}

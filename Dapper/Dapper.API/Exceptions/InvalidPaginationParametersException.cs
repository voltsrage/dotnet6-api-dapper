namespace Dapper.API.Exceptions
{
    /// <summary>
    /// Exception thrown when pagination input parameters are invalid
    /// </summary>
    public class InvalidPaginationParametersException : PaginationException
    {
        /// <summary>
        /// The invalid parameter that caused the exception
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        /// Creates a new invalid pagination parameters exception
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="component">Component where exception occurred</param>
        /// <param name="function">Function where exception occurred</param>
        /// <param name="table">Table being queried</param>
        /// <param name="parameterName">Name of the invalid parameter</param>
        /// <param name="innerException">Inner exception if available</param>
        /// <param name="additionalData">Additional contextual data</param>
        public InvalidPaginationParametersException(
            string message,
            string component,
            string function,
            string table,
            string parameterName,
            Exception innerException = null,
            Dictionary<string, object> additionalData = null)
            : base(message, component, function, table, innerException, additionalData)
        {
            ParameterName = parameterName;

            // Add parameter name to additional data for logging
            AdditionalData["parameterName"] = parameterName;
        }
    }
}

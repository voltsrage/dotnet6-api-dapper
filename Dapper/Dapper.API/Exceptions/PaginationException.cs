
namespace Dapper.API.Exceptions
{
    /// <summary>
    /// Base exception for pagination-related errors
    /// </summary>
    public class PaginationException : BaseException
    {
        /// <summary>
        /// The table or entity being queried when the exception occurred
        /// </summary>
        public string Table { get; }

        /// <summary>
        /// Creates a new pagination exception
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="component">Component where exception occurred</param>
        /// <param name="function">Function where exception occurred</param>
        /// <param name="table">Table being queried</param>
        /// <param name="innerException">Inner exception if available</param>
        /// <param name="additionalData">Additional contextual data</param>
        public PaginationException(
            string message,
            string component,
            string function,
            string table,
            Exception innerException = null,
            Dictionary<string, object> additionalData = null)
            : base(message, "PAGINATION_ERROR", component, function, innerException, additionalData)
        {
            Table = table;
        }
    }
}

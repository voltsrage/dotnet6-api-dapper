using System.Diagnostics;

namespace Dapper.API.Exceptions
{
    /// <summary>
    /// Base exception for all exceptions 
    /// </summary>
    public class BaseException : Exception
    {
        /// <summary>
        /// The error code for the exception
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// The component that the exception occurred in
        /// </summary>
        public string Component { get; }

        /// <summary>
        /// The function that the exception occurred in
        /// </summary>
        public string Function { get; }

        /// <summary>
        /// The trace ID for the exception
        /// </summary>
        public string TraceId { get; }

        /// <summary>
        /// Optional additional data for the exception
        /// </summary>
        public Dictionary<string, object> AdditionalData { get; }

        /// <summary>
        /// Used to create an instance of the AuthBaseException
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        /// <param name="component"></param>
        /// <param name="function"></param>
        /// <param name="innerException"></param>
        /// <param name="additionalData"></param>
        protected BaseException(
            string message,
            string errorCode,
            string component,
            string function,
            Exception innerException = null,
            Dictionary<string, object> additionalData = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            Component = component;
            Function = function;
            TraceId = Activity.Current?.Id ?? Guid.NewGuid().ToString();
            AdditionalData = additionalData ?? new Dictionary<string, object>();
        }
    }
}

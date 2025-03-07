namespace Dapper.API.Models
{
    /// <summary>
    /// Provides detailed information about an error that occurred in the application.
    /// Used for structured error responses in API calls.
    /// </summary>
    public class ErrorDetail
    {
        /// <summary>
        /// A unique code identifying the error type, often corresponding to system error codes.
        /// Can be used for error classification and client-side handling logic.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// A human-readable description of the error.
        /// Should be clear and informative without exposing sensitive system details.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The component or module where the error occurred.
        /// Helps in identifying the subsystem responsible for the error.
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// The specific function or operation that was being performed when the error occurred.
        /// Provides context for where in the execution flow the error happened.
        /// </summary>
        public string Function { get; set; }

        /// <summary>
        /// A dictionary containing additional context-specific information about the error.
        /// Can include timestamps, request IDs, or other diagnostic information.
        /// </summary>
        public Dictionary<string, object> AdditionalData { get; set; }

        /// <summary>
        /// A collection of validation errors when the error is related to invalid input.
        /// Each error specifies which property failed validation and why.
        /// Only populated for validation-related errors.
        /// </summary>
        public IReadOnlyList<ValidationError> ValidationErrors { get; set; }
    }
}

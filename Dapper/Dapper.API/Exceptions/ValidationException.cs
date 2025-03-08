using Dapper.API.Models;

namespace Dapper.API.Exceptions
{
    /// <summary>
    /// Validation exception
    /// </summary>
    public class ValidationException : BaseException
    {
        /// <summary>
        /// The validation errors
        /// </summary>
        public IReadOnlyList<ValidationError> Errors { get; }

        /// <summary>
        /// Used to create a new instance of the <see cref="ValidationException"/> class
        /// </summary>
        /// <param name="message"></param>
        /// <param name="validator"></param>
        /// <param name="function"></param>
        /// <param name="errors"></param>
        /// <param name="innerException"></param>
        public ValidationException(
            string message,
            string validator,
            string function,
            IEnumerable<ValidationError> errors,
            Exception innerException = null)
            : base(message, "VALIDATION_ERROR", validator, function, innerException)
        {
            Errors = errors?.ToList().AsReadOnly() ?? new List<ValidationError>().AsReadOnly();
        }
    }
}

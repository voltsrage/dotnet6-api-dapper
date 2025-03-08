namespace Dapper.API.Models
{
    /// <summary>
    /// Represents a validation error for a specific property in a model or entity.
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// The name of the property that failed validation.
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// The validation error message describing why the property is invalid.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// The value that was attempted to be assigned to the property but failed validation.
        /// This helps in debugging by showing what was actually submitted.
        /// </summary>
        public object AttemptedValue { get; set; }
    }
}

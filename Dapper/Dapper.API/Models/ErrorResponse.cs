namespace Dapper.API.Models
{
    /// <summary>
    /// Error response model
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Error details
        /// </summary>
        public ErrorDetail Error { get; set; }
        public string TraceId { get; set; }
        public long Timestamp { get; set; }
    }
}

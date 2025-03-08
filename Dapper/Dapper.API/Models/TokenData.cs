namespace Dapper.API.Models
{
    /// <summary>
    /// Properties returned in the token
    /// </summary>
    public class TokenData
    {
        /// <summary>
        /// Gets or sets the user's email address. This is often used as the username for login.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Adtional properties that can be added to the token.
        /// </summary>

        public Dictionary<string, object>? AdditionalProperties { get; set; }
    }
}

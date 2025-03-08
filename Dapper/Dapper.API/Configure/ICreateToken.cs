using Dapper.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Dapper.API.Configure
{
    /// <summary>
    /// Interface for token creation and validation operations.
    /// Provides methods to create, read, and validate JWT tokens for authentication and authorization.
    /// </summary>
    public interface ICreateToken
    {
        /// <summary>
        /// Creates a JWT token for a user with optional custom claims data.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom the token is created.</param>
        /// <param name="data">Optional additional data to include in the token claims.</param>
        /// <returns>A JWT token string that can be used for authentication.</returns>
        string CreateTokenMethod(string userId, TokenData? data = null);

        /// <summary>
        /// Creates a JWT token for a user on a specific device with optional custom claims data.
        /// Useful for scenarios requiring device-specific authentication.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom the token is created.</param>
        /// <param name="deviceId">The identifier of the device from which the user is authenticating.</param>
        /// <param name="data">Optional additional data to include in the token claims.</param>
        /// <returns>A JWT token string that can be used for authentication.</returns>
        string CreateTokenMethod(string userId, int? deviceId, TokenData? data = null);

        /// <summary>
        /// Extracts the claims principal from a JWT token.
        /// This can be used to access the claims within the token for authorization decisions.
        /// </summary>
        /// <param name="token">The JWT token string to parse.</param>
        /// <returns>A ClaimsPrincipal object containing the claims from the token.</returns>
        ClaimsPrincipal GetPrincipal(string token);

        /// <summary>
        /// Decodes a JWT token to access its raw information without validating it.
        /// </summary>
        /// <param name="token">The JWT token string to read.</param>
        /// <returns>A JwtSecurityToken object representing the decoded token.</returns>
        JwtSecurityToken ReadToken(string token);

        /// <summary>
        /// Validates a JWT token to ensure it is properly signed and not expired.
        /// </summary>
        /// <param name="token">The JWT token string to validate.</param>
        /// <returns>
        /// A tuple containing:
        /// - isValid: Boolean indicating if the token is valid
        /// - userId: The user ID extracted from the token if valid
        /// </returns>
        Task<(bool isValid, int userId)> ValidateToken(string token);
    }
}

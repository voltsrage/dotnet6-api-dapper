using Dapper.API.Enums;

namespace Dapper.API.Exceptions
{
    public class AuthenticationException : BaseException
    {
        public int UserId { get; }
        public SystemCodeEnum ErrorType { get; }

        public AuthenticationException(
            string message,
            string component,
            string function,
            int userId,
            SystemCodeEnum errorType,
            Exception innerException = null)
            : base(message, $"AUTH_{errorType}", component, function, innerException)
        {
            UserId = userId;
            ErrorType = errorType;
        }
    }
}

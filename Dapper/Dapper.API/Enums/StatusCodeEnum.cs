using Dapper.API.Enums.EnumBase;

namespace Dapper.API.Enums
{
    public class StatusCodeEnum : Enumeration<StatusCodeEnum>
    {
        // Successful 2xx
        public static readonly StatusCodeEnum OK = new StatusCodeEnum(200, "OK");
        public static readonly StatusCodeEnum Created = new StatusCodeEnum(201, "Created");
        public static readonly StatusCodeEnum NoContent = new StatusCodeEnum(204, "No Content ");
        public static readonly StatusCodeEnum PartialContent = new StatusCodeEnum(206, "Partial Content ");
        // Add other success codes as needed

        // Client Error 4xx
        public static readonly StatusCodeEnum BadRequest = new StatusCodeEnum(400, "Bad Request");
        public static readonly StatusCodeEnum Unauthorized = new StatusCodeEnum(401, "Unauthorized");
        public static readonly StatusCodeEnum PaymentRequired = new StatusCodeEnum(402, "Payment Required ");
        public static readonly StatusCodeEnum Forbidden = new StatusCodeEnum(403, "Forbidden");
        public static readonly StatusCodeEnum NotFound = new StatusCodeEnum(404, "Not Found");
        public static readonly StatusCodeEnum MethodNotAllowed = new StatusCodeEnum(405, "Bad Request");
        public static readonly StatusCodeEnum PayloadTooLarge = new StatusCodeEnum(413, "Unauthorized");
        public static readonly StatusCodeEnum TooManyRequests = new StatusCodeEnum(429, "Payment Required ");
        public static readonly StatusCodeEnum RequestHeaderFieldsTooLarge = new StatusCodeEnum(431, "Forbidden");
        public static readonly StatusCodeEnum UnavailableForLegalReasons = new StatusCodeEnum(451, "Not Found");
        // Add other client error codes as needed

        // Server Error 5xx
        public static readonly StatusCodeEnum InternalServerError = new StatusCodeEnum(500, "Internal Server Error");
        public static readonly StatusCodeEnum NotImplemented = new StatusCodeEnum(501, "Not Implemented");
        public static readonly StatusCodeEnum BadGateway = new StatusCodeEnum(502, "Bad Gateway");
        public static readonly StatusCodeEnum ServiceUnavailable = new StatusCodeEnum(503, "Service Unavailable");
        public static readonly StatusCodeEnum GatewayTimeout = new StatusCodeEnum(504, "Gateway Timeout");
        // Add other server error codes as needed

        private StatusCodeEnum(int value, string name) : base(value, name) { }
    }
}

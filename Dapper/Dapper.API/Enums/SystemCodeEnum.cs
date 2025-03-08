using Dapper.API.Enums.EnumBase;
using System.Net;

namespace Dapper.API.Enums
{
    /// <summary>
    /// Allows developer to create error codes for system logic in a central place
    /// </summary>
    public class SystemCodeEnum : Enumeration<SystemCodeEnum>
    {

        // e.g.
        //public static SystemCodeEnum UnknownError = new(1000, "Unknown Error", HttpStatusCode.InternalServerError);

        private SystemCodeEnum(int value, string name) : base(value, name) { }

        public HttpStatusCode StatusCode { get; private set; }

        private SystemCodeEnum(int value, string name, HttpStatusCode statusCode) : base(value, name)
        {
            StatusCode = statusCode;
        }
    }
}

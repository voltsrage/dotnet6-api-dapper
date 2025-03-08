using Dapper.API.Enums;

namespace Dapper.API.Models
{
    /// <summary>
    /// Response wrapper for API Endpoints
    /// </summary>
    public class Response
    {
        public Response()
        {
            IsSuccess = false;  //預設是false
        }

        /// <summary>
        /// 是否成功回傳
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// IsSuccess為false，顯示錯誤訊息
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// IsSuccess為true，顯示訊息
        /// </summary>
        public string? SuccessMessage { get; set; }

        /// <summary>
        /// Custom error code
        /// </summary>
        public int? ErrorCode { get; set; }

        /// <summary>
        /// Http Status Code
        /// </summary>
        public int? StatusCode { get; set; }



    }
    /// <summary>
    /// 回傳資料格式，泛型
    /// </summary>
    /// <typeparam name="T">資料內容</typeparam>
    public class Response<T> : Response
    {
        /// <summary>
        /// 資料內容
        /// </summary>
        public T? Content { get; set; }

        public static Response<T> Success(T content, string message = null)
        {
            return new Response<T>
            {
                IsSuccess = true,
                Content = content,
                SuccessMessage = message,
                StatusCode = StatusCodeEnum.OK.Value
            };
        }

        public static Response<T> Failure(SystemCodeEnum error, T? content = default)
        {
            return new Response<T>
            {
                Content = content,
                IsSuccess = false,
                ErrorMessage = error.Name,
                ErrorCode = error.Value,
                StatusCode = (int)error.StatusCode
            };
        }

        public static Response<T> Failure(string errorMessage, int errorCode, int statusCode)
        {
            return new Response<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode,
                StatusCode = statusCode
            };
        }

    }
}

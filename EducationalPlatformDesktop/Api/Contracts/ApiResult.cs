using System.Net;

namespace EducationalPlatformDesktop.Api.Contracts
{
    public sealed class ApiResult<T>
    {
        public bool IsSuccess { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public T? Data { get; set; }

        public string Message { get; set; } = string.Empty;

        public static ApiResult<T> Success(T data, HttpStatusCode statusCode)
        {
            return new ApiResult<T>
            {
                IsSuccess = true,
                StatusCode = statusCode,
                Data = data
            };
        }

        public static ApiResult<T> Error(HttpStatusCode statusCode, string message)
        {
            return new ApiResult<T>
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Message = message
            };
        }
    }
}
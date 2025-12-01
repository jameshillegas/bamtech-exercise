using System.Net;
using FluentValidation;

namespace StargateAPI.Controllers
{
    public class BaseResponse<T> where T : class
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "Successful";
        public int ResponseCode { get; set; } = (int)HttpStatusCode.OK;
        public T? Data { get; set; }

        public static BaseResponse<T> Ok(T data)
        {
            return new BaseResponse<T>
            {
                Data = data
            };
        }

        public static BaseResponse<T> NotFound(string? message = null)
        {
            return new BaseResponse<T>
            {
                Success = false,
                Message = message ?? "Not Found",
                ResponseCode = (int)HttpStatusCode.NotFound,
                Data = null
            };
        }

        public static BaseResponse<T> ServerError(string message)
        {
            return new BaseResponse<T>
            {
                Success = false,
                Message = message,
                ResponseCode = (int)HttpStatusCode.InternalServerError,
                Data = null
            };
        }

        public static BaseResponse<T> BadRequest(string message)
        {
            return new BaseResponse<T>
            {
                Success = false,
                Message = message,
                ResponseCode = (int)HttpStatusCode.BadRequest,
                Data = null
            };
        }
    }

    public static class ExceptionExtensions
    {
        public static BaseResponse<T> ToBadRequestResponse<T>(this ValidationException vex) where T : class
        {
            var errorMessage = string.Join("; ", vex.Errors.Select(e => e.ErrorMessage));
            return BaseResponse<T>.BadRequest(errorMessage);
        }
    }
}
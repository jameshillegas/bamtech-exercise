using Microsoft.AspNetCore.Mvc;

namespace StargateAPI.Controllers
{
    public static class ControllerBaseExtensions
    {

        public static IActionResult GetResponse<T>(this ControllerBase controllerBase, BaseResponse<T> response) where T : class
        {
            var httpResponse = new ObjectResult(response);
            httpResponse.StatusCode = response.ResponseCode;
            return httpResponse;
        }
    }
}
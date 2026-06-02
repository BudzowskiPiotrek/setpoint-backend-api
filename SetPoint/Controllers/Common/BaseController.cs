using Microsoft.AspNetCore.Mvc;
using SetPoint.API.Common;


namespace SetPoint.API._2.Controllers.Common
{
    public class BaseController : ControllerBase
    {
        protected IActionResult SuccessResponse(object? data, string message = "Success", int statusCode = 200)
        {
            return StatusCode(statusCode, new ApiResponse
            {
                Result = data,
                Message = message,
                StatusCode = statusCode
            });
        }

        protected IActionResult ErrorResponse(Exception ex, string customMessage)
        {
            return StatusCode(500, new ApiResponse
            {
                WithError = true,
                Message = $"{customMessage}: {ex.Message}",
                StatusCode = 500
            });
        }
    }

}


using Microsoft.AspNetCore.Mvc;

namespace MyTemplate.APIResponses;

public static class ControllerBaseExtension
{
    public static IActionResult ToActionResult<T>(this ControllerBase controller, Response<T> response)
    {

        if (response == null)
            return controller.StatusCode(StatusCodes.Status500InternalServerError, new { Code = "NULL_RESPONSE", Message = "Null response object" });


        if (response.IsSuccess)
            return controller.Ok(response);

        var error = response.Error;
        if (error == null)
            return controller.StatusCode(StatusCodes.Status500InternalServerError, new { Code = "UNKNOWN_ERROR", Message = "An unknown error occurred." });



        var statusCode = response.Error?.StatusCode ?? StatusCodes.Status500InternalServerError;
        return controller.StatusCode(statusCode, response);


    }
}

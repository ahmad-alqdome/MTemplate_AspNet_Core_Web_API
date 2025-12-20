using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyTemplate.APIResponses;

namespace MyTemplate.Filters;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // Modify  Fluent validation respone to be wraper response  with all errors
        if (!context.ModelState.IsValid)
        {
            var results = string.Join(", ",context.ModelState.SelectMany(kvp=>kvp.Value.Errors.Select(x=>$"{x.ErrorMessage}")));
            var response = Response.Failure<string>(new Error("VALIDATION_ERROR", results,StatusCodes.Status400BadRequest));


        context.Result=new JsonResult(response) { StatusCode=StatusCodes.Status400BadRequest};
        }


    }
}

using DAL.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web_API.Helpers;

public class ExceptionHadlerFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is DataValidationException)
        {
            context.Result = new BadRequestObjectResult(new { error = context.Exception.Message });
        }
        else if (context.Exception is DataAccessErrorException)
        {
            context.Result = new BadRequestObjectResult(new { error = context.Exception.Message });
        }
        else if(context.Exception is ObjectNotFoundException)
        {
            context.Result = new NotFoundObjectResult(new { error = context.Exception.Message });
        }
        // else
        // {
        //     context.Result = new BadRequestObjectResult(new { error = "An error occurred while processing your request" });
        // }
    }
}
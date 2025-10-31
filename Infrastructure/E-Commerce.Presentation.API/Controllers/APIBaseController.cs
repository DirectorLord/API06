using E_Commerce.Service.Abstraction.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace E_Commerce.Presentation.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class APIBaseController : ControllerBase
{
    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess) return NoContent();
        return Problem(result.Errors);
    }
    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess) return Ok(result.Value);
        return Problem(result.Errors);
    }
    private ActionResult Problem(IReadOnlyList<Error> errors)
    {
        if(errors.Count == 0) return Problem(statusCode: 500, title: "An Unexpected error");
        if (errors.All(e => e.Type == ErrorType.Validation)) return HandleValidationProblem(errors);
        return HandleSingleErrorProblem(errors[0]);
    }
    private ActionResult HandleSingleErrorProblem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };
        return Problem(statusCode: statusCode, title: error.Description, type: error.Code);
    }
    private ActionResult HandleValidationProblem(IReadOnlyList<Error> errors)
    {
        var modelState = new ModelStateDictionary();
        foreach (var error in errors)
        {
            modelState.AddModelError(error.Code, error.Description);
        }
        return ValidationProblem(modelState);
    }
}

using DiagCom.RestApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DiagCom.RestApi.Validators
{
    class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //invalid payload
            if (!context.ModelState.IsValid)
            {
                var errorsInModelState = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage)).ToArray();
                var errorResponse = new ErrorResponse();
                foreach (var error in errorsInModelState)
                {
                    foreach (var subError in error.Value ?? Array.Empty<string>())
                    {
                        errorResponse.Message.Add($"Field: {error.Key}, {subError}");
                    }
                }

                context.Result = new BadRequestObjectResult(errorResponse);
                errorResponse.Status = StatusCodes.Status400BadRequest;
                return;
            }
            await next();
        }
    }
}

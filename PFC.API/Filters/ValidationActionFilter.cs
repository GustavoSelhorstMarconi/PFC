using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PFC.API.Filters;

public class ValidationActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var argument in context.ActionArguments)
        {
            var argValue = argument.Value;
            if (argValue is null)
                continue;

            var argType = argValue.GetType();

            var validatorType = typeof(IValidator<>).MakeGenericType(argType);
            dynamic validator = context.HttpContext.RequestServices.GetService(validatorType);

            if (validator == null)
                continue;

            var contextType = typeof(ValidationContext<>).MakeGenericType(argType);
            dynamic validationContext = Activator.CreateInstance(contextType, argValue);

            ValidationResult result = validator.Validate(validationContext);

            if (!result.IsValid)
            {
                context.Result = new BadRequestObjectResult(result.Errors);
                return;
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}

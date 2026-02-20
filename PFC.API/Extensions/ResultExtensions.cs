using Microsoft.AspNetCore.Mvc;
using PFC.Application.Common;

namespace PFC.API.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToOkActionResult<T>(this Result<T> result)
    {
        if (result.IsFailure)
            throw new InvalidOperationException("Cannot convert failed result to Ok. Use middleware to handle failures.");

        return new OkObjectResult(result.Value);
    }

    public static IActionResult ToCreatedActionResult<T>(
        this Result<T> result,
        string actionName,
        object? routeValues = null)
    {
        if (result.IsFailure)
            throw new InvalidOperationException("Cannot convert failed result to Created. Use middleware to handle failures.");

        return new CreatedAtActionResult(actionName, null, routeValues, result.Value);
    }

    public static IActionResult ToCreatedActionResult<T>(
        this Result<T> result,
        string uri)
    {
        if (result.IsFailure)
            throw new InvalidOperationException("Cannot convert failed result to Created. Use middleware to handle failures.");

        return new CreatedResult(uri, result.Value);
    }

    public static IActionResult ToNoContentActionResult(this Result result)
    {
        if (result.IsFailure)
            throw new InvalidOperationException("Cannot convert failed result to NoContent. Use middleware to handle failures.");

        return new NoContentResult();
    }

    public static IActionResult ToNoContentActionResult<T>(this Result<T> result)
    {
        if (result.IsFailure)
            throw new InvalidOperationException("Cannot convert failed result to NoContent. Use middleware to handle failures.");

        return new NoContentResult();
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsFailure)
            return new BadRequestObjectResult(new { error = result.Error });

        if (result.Value is null)
            return new NoContentResult();

        return new OkObjectResult(result.Value);
    }

    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsFailure)
            return new BadRequestObjectResult(new { error = result.Error });

        return new NoContentResult();
    }
}

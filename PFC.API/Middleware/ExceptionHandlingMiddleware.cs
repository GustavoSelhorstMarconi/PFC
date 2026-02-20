using PFC.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace PFC.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        var (statusCode, response) = exception switch
        {
            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                new ErrorResponse
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Title = "Resource Not Found",
                    Detail = notFoundEx.Message,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
                }),

            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                new ValidationErrorResponse
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Validation Error",
                    Detail = validationEx.Message,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Errors = validationEx.Errors
                }),

            BadRequestException badRequestEx => (
                HttpStatusCode.BadRequest,
                new ErrorResponse
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Bad Request",
                    Detail = badRequestEx.Message,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                }),

            BusinessException businessEx => (
                HttpStatusCode.BadRequest,
                new ErrorResponse
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Business Rule Violation",
                    Detail = businessEx.Message,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                }),

            UnauthorizedException unauthorizedEx => (
                HttpStatusCode.Unauthorized,
                new ErrorResponse
                {
                    Status = (int)HttpStatusCode.Unauthorized,
                    Title = "Unauthorized",
                    Detail = unauthorizedEx.Message,
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
                }),

            ForbiddenException forbiddenEx => (
                HttpStatusCode.Forbidden,
                new ErrorResponse
                {
                    Status = (int)HttpStatusCode.Forbidden,
                    Title = "Forbidden",
                    Detail = forbiddenEx.Message,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
                }),

            ConflictException conflictEx => (
                HttpStatusCode.Conflict,
                new ErrorResponse
                {
                    Status = (int)HttpStatusCode.Conflict,
                    Title = "Conflict",
                    Detail = conflictEx.Message,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
                }),

            _ => (
                HttpStatusCode.InternalServerError,
                new ErrorResponse
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred. Please try again later.",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
                })
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(response, response.GetType(), options);
        await context.Response.WriteAsync(json);
    }
}

public class ErrorResponse
{
    public int Status { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class ValidationErrorResponse : ErrorResponse
{
    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
}

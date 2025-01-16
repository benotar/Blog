using System.Text.Json;
using Blog.Application.Common;
using Blog.Domain.Enums;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly JsonSerializerOptions _options;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, JsonSerializerOptions options)
    {
        _logger = logger;
        _options = options;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, exception.Message);

        var details = new ProblemDetails
        {
            Type = "Server Error",
            Title = "Unexpected API Error",
            Detail = "One or more unexpected errors occurred.",
            Status = StatusCodes.Status500InternalServerError,
            Instance = httpContext.Request.Path
        };

        var result = new Result<ProblemDetails>
        {
            ErrorCode = ErrorCode.UnexpectedError,
            Payload = details
        };

        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(result, _options),
            cancellationToken);

        return true;
    }
}
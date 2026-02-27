using MiniTasksSystem.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MiniTasksSystem.Api.Middleware;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;
    private readonly IProblemDetailsService _problemDetailsService = problemDetailsService;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;

        _logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}", traceId);

        ProblemDetailsContext context = new()
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = CreateProblemDetails(exception)
        };
        context.ProblemDetails.Extensions["traceId"] = traceId;

        return await _problemDetailsService.TryWriteAsync(context);
    }

    private static ProblemDetails CreateProblemDetails(Exception exception) => exception switch
    {
        ValidationException validationEx => new()
        {
            Title = "Validation Error",
            Detail = validationEx.Message,
            Status = StatusCodes.Status400BadRequest,
            Extensions =
            {
                ["errors"] = validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key.ToLowerInvariant(),
                        g => g.Select(e => e.ErrorMessage).ToArray())
            }
        },

        NotFoundException notFoundEx => new()
        {
            Title = "Not Found",
            Status = StatusCodes.Status404NotFound,
            Detail = notFoundEx.Message
        },

        UnauthorizedAccessException unauthorizedEx => new()
        {
            Title = "Unauthorized",
            Status = StatusCodes.Status401Unauthorized,
            Detail = unauthorizedEx.Message
        },

        InvalidOperationException invalidOpEx => new()
        {
            Title = "Conflict",
            Status = StatusCodes.Status409Conflict,
            Detail = invalidOpEx.Message
        },

        _ => new()
        {
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = "An unexpected error occurred."
        }
    };
}

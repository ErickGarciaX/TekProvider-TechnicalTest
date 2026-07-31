using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TekProvider.Application.Common.Exceptions;
using TekProvider.Domain.Exceptions;

namespace TekProvider.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title, errorCode) = MapException(exception);

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception");
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title
        };

        if (errorCode is not null)
        {
            problemDetails.Extensions["errorCode"] = errorCode;
        }

        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static (int StatusCode, string Title, string? ErrorCode) MapException(Exception exception) => exception switch
    {
        ValidationException => (StatusCodes.Status400BadRequest, "Validation failed.", "validation-failed"),
        InvalidStateTransitionException ex => (StatusCodes.Status400BadRequest, ex.Message, ex.ErrorCode),
        CustomerNotFoundException ex => (StatusCodes.Status404NotFound, ex.Message, ex.ErrorCode),
        DuplicateCustomerException ex => (StatusCodes.Status409Conflict, ex.Message, ex.ErrorCode),
        ConcurrencyConflictException ex => (StatusCodes.Status409Conflict, ex.Message, ex.ErrorCode),
        InvalidCredentialsException ex => (StatusCodes.Status401Unauthorized, ex.Message, ex.ErrorCode),
        UsernameAlreadyTakenException ex => (StatusCodes.Status409Conflict, ex.Message, ex.ErrorCode),
        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", null)
    };
}

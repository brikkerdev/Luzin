using System.Diagnostics;
using System.Text.Json;
using FluentValidation;
using MusicWeb.src.Exceptions;

namespace MusicWeb.src.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
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
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        var (statusCode, errorCode, message, validationErrors) = MapException(exception);

        LogException(exception, traceId, statusCode);

        var response = new ErrorResponse
        {
            Error = errorCode,
            Message = message,
            TraceId = traceId,
            ValidationErrors = validationErrors
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }

    private (int StatusCode, string ErrorCode, string Message, IDictionary<string, string[]>? ValidationErrors)
        MapException(Exception exception)
    {
        return exception switch
        {
            AppException appEx => (
                appEx.StatusCode,
                appEx.ErrorCode,
                appEx.Message,
                null
            ),

            ValidationException fluentEx => (
                StatusCodes.Status400BadRequest,
                "ValidationError",
                "One or more validation errors occurred.",
                fluentEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
            ),

            OperationCanceledException => (
                499, // Client Closed Request
                "RequestCancelled",
                "The request was cancelled.",
                null
            ),

            ArgumentNullException argNullEx => (
                StatusCodes.Status400BadRequest,
                "BadRequest",
                $"Required parameter '{argNullEx.ParamName}' was null.",
                null
            ),

            ArgumentException argEx => (
                StatusCodes.Status400BadRequest,
                "BadRequest",
                argEx.Message,
                null
            ),

            TimeoutException => (
                StatusCodes.Status408RequestTimeout,
                "RequestTimeout",
                "The request timed out.",
                null
            ),

            UnauthorizedAccessException => (
                StatusCodes.Status403Forbidden,
                "Forbidden",
                "Access denied.",
                null
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                "InternalServerError",
                _env.IsDevelopment() ? exception.Message : "An unexpected error occurred.",
                null
            )
        };
    }

    private void LogException(Exception exception, string traceId, int statusCode)
    {
        var logLevel = statusCode >= 500 ? LogLevel.Error : LogLevel.Warning;

        _logger.Log(
            logLevel,
            exception,
            "Exception: {ExceptionType} | TraceId: {TraceId} | Status: {StatusCode} | Message: {Message}",
            exception.GetType().Name,
            traceId,
            statusCode,
            exception.Message
        );
    }
}
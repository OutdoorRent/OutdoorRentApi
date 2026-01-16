namespace Identity.API.Middlewares;

using System.Net;
using Amazon.CognitoIdentityProvider.Model;
using OutdoorRent.Shared.Responses;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.OK;

        var response = exception switch
        {
            CodeMismatchException => ApiResponse.Fail(
                "AUTH_CODE_INVALID",
                "Invalid verification code provided, please try again."
            ),

            ExpiredCodeException => ApiResponse.Fail(
                "AUTH_CODE_EXPIRED",
                "Verification code expired"
            ),

            UserNotConfirmedException => ApiResponse.Fail(
                "AUTH_EMAIL_NOT_VERIFIED",
                "Email is not verified"
            ),

            _ => ApiResponse.Fail(
                "SYSTEM_ERROR",
                exception.Message
            )
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}

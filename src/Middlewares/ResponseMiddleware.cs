public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine($"[Request1] {context.Request.Method} {context.Request.Path}");
        
        // Call the next middleware in the pipeline
        await _next(context);

        Console.WriteLine($"[Response1] {context.Response.StatusCode}");
    }
}

namespace Catalog.API.Middleware;

public class AnotherMiddleware
{
    private readonly RequestDelegate _next;

    public AnotherMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine($"[Request2] {context.Request.Method} {context.Request.Path}");
        
        // Call the next middleware in the pipeline
        await _next(context);

        Console.WriteLine($"[Response2] {context.Response.StatusCode}");
    }
}

namespace OutdoorRent.Shared.Responses;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public string Code { get; init; } = default!;
    public string Message { get; init; } = default!;
    public T? Data { get; init; }

    public static ApiResponse<T> Ok(T data, string message = "OK")
        => new()
        {
            Success = true,
            Code = "OK",
            Message = message,
            Data = data
        };

    public static ApiResponse<T> Fail(string code, string message)
        => new()
        {
            Success = false,
            Code = code,
            Message = message
        };
}

public class ApiResponse
{
    public bool Success { get; set; }
    
    public string Code { get; init; } = default!;
    public string Message { get; init; } = default!;

    public static ApiResponse Ok()
        => new() { Success = true, Code = "OK", Message = "OK" };

    public static ApiResponse Fail(string code, string message)
        => new()
        {
            Success = false,
            Code = code,
            Message = message
        };
}

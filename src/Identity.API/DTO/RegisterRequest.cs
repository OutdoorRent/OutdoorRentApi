namespace Identity.API.DTO;

public class RegisterRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
}

public class RegisterResponse
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public bool RequiresConfirmation { get; set; }
}

public class VerifyEmailRequest
{
    public string Email { get; set; }
    public string Code { get; set; }
}

public class ResendVerifyCodeRequest
{
    public string Email { get; set; }
}
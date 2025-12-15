using Microsoft.AspNetCore.Mvc;
using Catalog.API.Services;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        // Dummy validation (replace with real user validation)
        if (model.Username == "admin" && model.Password == "password")
        {
            var token = _tokenService.GenerateToken(model.Username);
            return Ok(new { Token = token });
        }

        return Unauthorized();
    }
}

public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}
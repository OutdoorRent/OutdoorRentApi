using Identity.API.Services;
using Identity.API.Models;
using Microsoft.AspNetCore.Mvc;
using Duende.IdentityModel;
using Identity.API.DTO;
using OutdoorRent.Shared.Responses;

namespace Identity.API.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly CognitoAuthService _authService;
    private const string CodeVerifierSessionKey = "code_verifier";

    public AuthController(CognitoAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        var codeVerifier = CryptoRandom.CreateUniqueId(32);
        var codeChallenge = codeVerifier.ToSha256();

        HttpContext.Session.SetString(CodeVerifierSessionKey, codeVerifier);

        var loginUrl = _authService.GetLoginUrl(codeChallenge);
        return Redirect(loginUrl);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code)
    {
        if (string.IsNullOrEmpty(code))
            return BadRequest("Missing code");

        var codeVerifier = HttpContext.Session.GetString(CodeVerifierSessionKey);
        // if (string.IsNullOrEmpty(codeVerifier))
        //     return BadRequest("Missing code verifier");

        var token = await _authService.ExchangeCodeForTokenAsync(code, codeVerifier);

        HttpContext.Session.SetString("access_token", token.AccessToken);
        HttpContext.Session.SetString("id_token", token.IdToken);

        return Ok(token);
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest data)
    {
        
        var cognitoSub = await _authService.RegisterAsync(data);

        await _authService.CreateLocalUserAsync(cognitoSub, data);
        
        return Ok(
            ApiResponse<string>.Ok("register success!"));
    }
    
    [HttpPost("verify")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        await _authService.ConfirmSignUpAsync(request);
        return Ok(ApiResponse<string>.Ok("Email verified"));
    }
    
    [HttpPost("resend")]
    public async Task<IActionResult> ResendVerifyCode([FromBody] ResendVerifyCodeRequest request)
    {
        await _authService.ResendVerificationCodeAsync(request.Email);
        return Ok(ApiResponse<string>.Ok("Success resend verify code"));
    }
    
}
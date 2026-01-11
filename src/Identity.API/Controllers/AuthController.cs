using Identity.API.Services;
using Identity.API.Models;
using Microsoft.AspNetCore.Mvc;
using Duende.IdentityModel;

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
}
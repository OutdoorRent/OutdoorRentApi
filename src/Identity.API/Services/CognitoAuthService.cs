using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Duende.IdentityModel;
using Duende.IdentityModel.Client;
using Identity.API.Models;
using Identity.API.DTO;
using Identity.API.Data;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;

namespace Identity.API.Services;

public class CognitoAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly IdentityDbContext _db;
    private readonly IAmazonCognitoIdentityProvider _cognitoClient;

    public CognitoAuthService(HttpClient httpClient, IConfiguration config, IdentityDbContext db, IAmazonCognitoIdentityProvider cognitoClient)
    {
        _httpClient = httpClient;
        _config = config;
        _db = db;
        _cognitoClient = cognitoClient;
    }

    public string GetLoginUrl(string codeChallenge)
    {
        var loginPage = _config["Cognito:LoginPage"];
        var clientId = _config["Cognito:ClientId"];
        var redirectUri = _config["Cognito:RedirectUri"];
        // var scopes = _config["Cognito:Scopes"];

        return $"{loginPage}/login?" +
               $"client_id={clientId}" +
               $"&response_type=code" +
               $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
               $"&scope=openid+profile+email+phone";
        // +
        // $"&code_challenge={codeChallenge}" +
        // $"&code_challenge_method=S256";
    }


    public async Task<Models.TokenResponse> ExchangeCodeForTokenAsync(string code, string codeVerifier)
    {
        // var authority = _config["Cognito:Authority"];
        // var clientId = _config["Cognito:ClientId"];
        // var redirectUri = _config["Cognito:RedirectUri"];
        //
        // var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        // {
        //     Address = authority,
        //     Policy =
        //     {
        //         // ValidateIssuerName = true,
        //         ValidateEndpoints = false
        //     }
        // });
        //
        // if (disco.IsError)
        //     throw new Exception(disco.Error);
        //
        // var tokenRequest = new AuthorizationCodeTokenRequest
        // {
        //     Address = disco.TokenEndpoint,
        //     ClientId = clientId,
        //     ClientSecret = clientSecret,
        //     Code = code,
        //     RedirectUri = redirectUri,
        //     CodeVerifier = codeVerifier,
        //     GrantType = OidcConstants.GrantTypes.AuthorizationCode,
        // };
        //
        // var tokenResponse = await _httpClient.RequestAuthorizationCodeTokenAsync(tokenRequest);
        //
        // if (tokenResponse.IsError)
        //     throw new Exception(tokenResponse.Error);
        //
        var domain = _config["Cognito:Domain"];
        var clientId = _config["Cognito:ClientId"];
        // var clientSecret = _config["Cognito:ClientSecret"];
        var redirectUri = _config["Cognito:RedirectUri"];

        using var client = new HttpClient();

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{domain}/oauth2/token"
        );

        // Basic Auth（如果是 public client 可去掉）
        // var auth = Convert.ToBase64String(
        //     Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")
        // );

        // request.Headers.Authorization =
        //     new AuthenticationHeaderValue("Basic", auth);

        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["client_id"] = clientId,
            // ["client_secret"] = clientSecret,
            ["code"] = code,
            ["redirect_uri"] = redirectUri,
            // ["code_verifier"] = codeVerifier
        });

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<Models.TokenResponse>(json)!;

        // return tokenResponse;
    }

    public async Task<string> RegisterAsync(RegisterRequest request)
    {

        var signUpRequest = new SignUpRequest
        {
            ClientId = _config["Cognito:ClientId"],
            Username = request.Email,
            Password = request.Password,
            UserAttributes = new List<AttributeType>
            {
                new AttributeType
                {
                    Name = "email",
                    Value = request.Email
                },
                new AttributeType
                {
                    Name = "name",
                    Value = request.FullName
                }
            }
        };

        var response = await _cognitoClient.SignUpAsync(signUpRequest);

        return response.UserSub; 
    }
    
    public async Task CreateLocalUserAsync(
        string cognitoSub,
        RegisterRequest request)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            CognitoSub = cognitoSub,
            Email = request.Email,
            FullName = request.FullName,
            Role = UserRole.Customer,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }
    
    public async Task ConfirmSignUpAsync(VerifyEmailRequest body)
    {
        var request = new ConfirmSignUpRequest
        {
            ClientId = _config["Cognito:ClientId"],
            Username = body.Email,
            ConfirmationCode = body.Code
        };
        await _cognitoClient.ConfirmSignUpAsync(request);
    }
    
    public async Task ResendVerificationCodeAsync(string email)
    {
        var request = new ResendConfirmationCodeRequest
        {
            ClientId = _config["Cognito:ClientId"],
            Username = email
        };
        await _cognitoClient.ResendConfirmationCodeAsync(request);
    }
}

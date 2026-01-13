using System.Security.Claims;
using Amazon;
using Identity.API.Data;
using Microsoft.EntityFrameworkCore;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Identity.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// AWS configuration
var awsOptions = new AWSOptions
{
    Region = RegionEndpoint.GetBySystemName(
        builder.Configuration["AWS:Region"]
    ),
    Credentials = new BasicAWSCredentials(
        builder.Configuration["AWS:Credentials:AccessKey"],
        builder.Configuration["AWS:Credentials:SecretKey"]
    )
};

builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonCognitoIdentityProvider>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["Cognito:Authority"];
        options.TokenValidationParameters = new()
        {
            ValidateAudience = false, 
            NameClaimType = "username"
        };
    });
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization();
builder.Services.AddHttpClient<Identity.API.Services.CognitoAuthService, Identity.API.Services.CognitoAuthService>();

builder.Services.AddDbContext<IdentityDbContext>(options =>
    // options.UsePostgresQL(builder.Configuration.GetConnectionString("DefaultConnection"))
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapGet("/secure", (ClaimsPrincipal user) =>
    {
        return new
        {
            user.Identity?.Name,
            Claims = user.Claims.Select(c => new { c.Type, c.Value })
        };
    })
    .RequireAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
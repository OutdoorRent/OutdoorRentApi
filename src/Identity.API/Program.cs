using System.Security.Claims;
using Identity.API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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
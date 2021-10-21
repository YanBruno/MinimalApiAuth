using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Services;
using MinimalApiAuth;
using MinimalApiAuth.Models;
using MinimalApiAuth.Repository;

var builder = WebApplication.CreateBuilder(args);
var key = Encoding.ASCII.GetBytes(Settings.SecretTokenKey);

builder.Services.AddAuthentication(
    options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
)
    .AddJwtBearer(
        options => {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters{
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        }
    );

builder.Services.AddAuthorization(
    options => {
        options.AddPolicy("Admin", policy => policy.RequireRole("manager"));
        options.AddPolicy("Employee", policy => policy.RequireRole("employee"));
    }
);   

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", async (User model) => {
    var user = await UserRepository.GetUser(model.Username, model.Password);
    if(user == null)
        return Results.NotFound(new {
            message = "Usuário não encontrado"
        });

    var token = TokenService.GenerateToken(user);
    user.Password = "null";

    return Results.Ok(new {
        user,
        token
    });
});

app.MapGet("/anonymous", ()=>{ Results.Ok("Anônimo"); }).AllowAnonymous();

app.MapGet("/authenticated", (ClaimsPrincipal user)=>{
     Results.Ok($"{user.Identity.Name}"); 
}).RequireAuthorization();

app.MapGet("/employee", (ClaimsPrincipal user)=>{
     Results.Ok($"{user.Identity.Name}"); 
}).RequireAuthorization("Employee");

app.MapGet("/manager", (ClaimsPrincipal user)=>{
     Results.Ok($"Manager - {user.Identity.Name}"); 
}).RequireAuthorization("Admin");

app.Run();

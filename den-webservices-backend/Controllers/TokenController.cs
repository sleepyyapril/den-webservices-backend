using System.Security.Claims;
using DenWebServices.Backend.Config;
using DenWebServices.Backend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace DenWebServices.Backend.Controllers;

[ApiController]
[Route("/token")]
public class TokenController(IConfiguration configuration, IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<IResult> GetToken()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded) return Results.Unauthorized();
        
        var jwtSettings = configuration.GetSection(Program.JwtSettingsSection).Get<JwtSettings>();
        if (jwtSettings is null) throw new InvalidOperationException("JwtSettings is not configured");

        var claims = result.Principal.Claims.ToList();
        
        // Create JWT token
        var tokenString = Util.GenerateJwt(jwtSettings, claims);
        // await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var session = new CurrentSession(tokenString, result);
        var loginResult = await userService.LoginAsync(session);
        var rankDisplay = loginResult.Rank != null ? loginResult.Rank.Name : "No rank";
        return Results.Ok(
            new { token = tokenString, email = loginResult.Email, rank = rankDisplay });
    }
}
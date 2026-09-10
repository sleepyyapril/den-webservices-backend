using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace DenWebServices.Backend.Controllers;

[ApiController]
[Route("/login")]
public class LoginController : ControllerBase
{
    [HttpGet]
    public IResult Login()
    {
        var properties = new AuthenticationProperties 
        { 
            RedirectUri = "/token",
            IsPersistent = true
        };
    
        // Triggers the OAuth challenge and redirects the user to Discord's authorization page
        return Results.Challenge(properties, [DiscordAuthenticationDefaults.AuthenticationScheme]);
    }
}
using System.Globalization;
using AspNet.Security.OAuth.Discord;
using DenWebServices.Backend.Config;
using DenWebServices.Backend.Database;
using DenWebServices.Backend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DenWebServices.Backend;

public static class Program
{
    private const string ConfigurationFilePath = "appsettings.json";
    private const string DiscordSettingsSection = "Discord";
    public const string JwtSettingsSection = "Jwt";
    private const string ListenSettingsSection = "Listen";
    
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Configuration
            .AddJsonFile(ConfigurationFilePath)
            .Build();
        
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.Configure<DiscordSettings>(builder.Configuration.GetSection(DiscordSettingsSection));
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettingsSection));
        builder.Services.Configure<ListenSettings>(builder.Configuration.GetSection(ListenSettingsSection));
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
            })
            .AddDiscord(options =>
            {
                var config = builder.Configuration.GetSection(DiscordSettingsSection).Get<DiscordSettings>();
                var scopes = config?.Scopes ?? [];
                options.ClientId = config?.ClientId ?? string.Empty;
                options.ClientSecret = config?.ClientSecret ?? string.Empty;
                options.CallbackPath = config?.Callback ?? "/signin-discord";
                options.SaveTokens = true;
                
                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                
                options.ClaimActions.MapCustomJson("urn:discord:avatar:url", user =>
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "https://cdn.discordapp.com/avatars/{0}/{1}.{2}",
                        user.GetString("id"),
                        user.GetString("avatar"),
                        user.GetString("avatar")!.StartsWith("a_") ? "gif" : "png"));

                foreach (var scope in scopes)
                {
                    options.Scope.Add(scope);
                }
                
                options.Events = new OAuthEvents
                {
                    OnTicketReceived = context =>
                    {
                        Console.WriteLine("Ticket received from Discord");
                        var claims = context.Principal?.Claims ?? [];
                        foreach (var claim in claims) Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
                        return Task.CompletedTask;
                    }
                };
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "den-webservices-session";
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            })
            .AddJwtBearer(options =>
            {
                var jwtSettings = builder.Configuration.GetSection(JwtSettingsSection).Get<JwtSettings>();
                
                if (jwtSettings is null) 
                    throw new InvalidOperationException("JwtSettings is not configured");

                var rsa = Util.LoadRsaKey(jwtSettings.RsaPublicKeyLocation);
                var listenSettings = builder.Configuration.Get<ListenSettings>();

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer ?? listenSettings?.Url ?? string.Empty,
                    ValidAudience = jwtSettings.Audience ?? listenSettings?.Url ?? string.Empty,
                    IssuerSigningKey = new RsaSecurityKey(rsa),
                    RequireSignedTokens = true
                };
            });
        
        builder.Services.AddControllers();
        
        var app = builder.Build();
        
        app.UseAuthentication();
        app.UseAuthorization();

        await using (var scope = app.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }
        
        var listen = builder.Configuration.GetSection(ListenSettingsSection).Get<ListenSettings>();

        app.MapControllers();
        app.Run(listen?.Url ?? "http://localhost:5000");
    }
}
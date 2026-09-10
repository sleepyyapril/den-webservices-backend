using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using DenWebServices.Backend.Config;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace DenWebServices.Backend;

public class Util
{
    public static RSA LoadRsaKey(string rsaKeyPath)
    {
        var rsa = RSA.Create();
        if (!File.Exists(rsaKeyPath))
        {
            throw new FileNotFoundException("RSA key file not found", rsaKeyPath);
        }
        var pemContents = File.ReadAllText(rsaKeyPath); 
        rsa.ImportFromPem(pemContents.ToCharArray());

        return rsa;
    }
    
    public static string GenerateJwt(JwtSettings jwtSettings, List<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var rsaPath = Path.GetFullPath(jwtSettings.RsaPrivateKeyLocation);
        var rsa = LoadRsaKey(rsaPath);
        var signingCredentials = new SigningCredentials(
            new RsaSecurityKey(rsa),
            SecurityAlgorithms.RsaSha256
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience,
            SigningCredentials = signingCredentials
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

public class CurrentSession
{
    public string SessionToken { get; set; }
    public long DiscordId { get; set; }
    public string Username { get; set; }
    public string EmailAddress { get; set; }
    public string AvatarUrl { get; set; }

    public CurrentSession(string sessionToken, AuthenticateResult result)
    {
        SessionToken = sessionToken;

        var claims = result.Principal?.Claims.ToList() ?? [];
        var discordId = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier,
            new Claim(ClaimTypes.NameIdentifier, "unknown")).Value;
        var username = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name,
            new Claim(ClaimTypes.Name, "unknown")).Value;
        var emailAddress = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email,
            new Claim(ClaimTypes.Email, "unknown")).Value;
        var avatarUrl = claims.FirstOrDefault(claim => claim.Type == "urn:discord:avatar:url",
            new Claim("urn:discord:avatar:url", "unknown")).Value;
        
        DiscordId = long.Parse(discordId);
        Username = username;
        EmailAddress = emailAddress;
        AvatarUrl = avatarUrl;
    }
}
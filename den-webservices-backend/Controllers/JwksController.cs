using DenWebServices.Backend.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DenWebServices.Backend.Controllers;

[ApiController]
[Route("/.well-known/jwks.json")]
public class JwksController : ControllerBase
{
    [HttpGet]
    public IResult Get(WebApplicationBuilder builder)
    {
        var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
        if (jwtOptions is null) throw new InvalidOperationException("JwtSettings is not configured");

        var rsaKey = Util.LoadRsaKey(jwtOptions.RsaPublicKeyLocation);
        var rsaParameters = rsaKey.ExportParameters(false);

        var jwk = new JsonWebKey
        {
            Kty = "RSA",
            E = Base64UrlEncoder.Encode(rsaParameters.Exponent),
            N = Base64UrlEncoder.Encode(rsaParameters.Modulus),
            Kid = "vasitos-public-key",
            Use = "sig",
            KeyOps = { "verify" },
            Alg = SecurityAlgorithms.RsaSha256
        };

        var jwks = new
        {
            Keys = new[] { jwk }
        };

        return Results.Json(jwks);
    }
}
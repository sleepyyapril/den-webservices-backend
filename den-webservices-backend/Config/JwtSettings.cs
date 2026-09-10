namespace DenWebServices.Backend.Config;

public class JwtSettings
{
    public required string RsaPrivateKeyLocation { get; set; }
    public required string RsaPublicKeyLocation { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
}
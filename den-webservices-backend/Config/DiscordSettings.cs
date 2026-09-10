namespace DenWebServices.Backend.Config;

public class DiscordSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Callback { get; set; } = string.Empty;
    public List<string> Scopes { get; set; } = ["identify", "email"];
}
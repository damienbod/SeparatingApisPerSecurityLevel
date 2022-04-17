namespace BlazorAuth0Bff.Server;

public class Auth0ApiConfiguration
{
    public string Audience { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
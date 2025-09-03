namespace SignalRHubDocs;
public class AuthScheme
{
    public string Name { get; set; } = string.Empty;
    public AuthType Type { get; set; }
    public string? HeaderName { get; set; }
    public string? QueryParamName { get; set; }
    public string? CookieName { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

public enum AuthType
{
    Bearer,
    ApiKey,
    QueryParam,
    Cookie,
    CustomHeader
}
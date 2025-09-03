using System.Collections.Generic;

namespace SignalRHubDocs;

public class SignalRDocumentation
{
    public string Title { get; set; } = "SignalR API Documentation";
    public string Version { get; set; } = "1.0.0";
    public string Description { get; set; }
    public List<HubInfo> Hubs { get; set; } = new();
    public string[] SupportedProtocols { get; set; } = { "json", "messagepack" };
    public List<AuthScheme> SupportedAuthSchemes { get; set; } = new();
}

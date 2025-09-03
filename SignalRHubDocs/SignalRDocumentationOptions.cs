using System.Collections.Generic;

namespace SignalRHubDocs;

public class SignalRDocumentationOptions
{
    public string RoutePrefix { get; set; } = "/signalr-docs";
    public string Title { get; set; } = "SignalR Hub Documentation";
    public string Version { get; set; } = "1.0.0";
    public string Description { get; set; } = "Auto-generated SignalR hub documentation";
    public bool SendEnumByString { get; set; } = false;
    public List<AuthScheme> SupportedAuthSchemes { get; set; } = new();
}

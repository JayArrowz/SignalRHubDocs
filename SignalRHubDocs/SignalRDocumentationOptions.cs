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
    public string SignalRClientVersion { get; set; } = "8.0.0";
    public string MessagePackClientVersion { get; set; } = "8.0.0";
    public string SignalRClientCdnUrl { get; set; } = "https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr";
    public string MessagePackClientCdnUrl { get; set; } = "https://cdn.jsdelivr.net/npm/@microsoft/signalr-protocol-msgpack";
    public string SignalRClientUrl => $"{SignalRClientCdnUrl}/{SignalRClientVersion}/signalr.min.js";
    public string MessagePackClientUrl => $"{MessagePackClientCdnUrl}@{MessagePackClientVersion}/dist/browser/signalr-protocol-msgpack.min.js";
}

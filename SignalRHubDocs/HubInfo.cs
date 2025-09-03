using System.Collections.Generic;

namespace SignalRHubDocs;

public class HubInfo
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public required string Route { get; set; }
    public List<HubMethodInfo> Methods { get; set; } = new();
    public bool RequiresAuth { get; set; }
    public string[] SupportedProtocols { get; set; } = { "json", "messagepack" };
    public List<string> SupportedAuthSchemes { get; set; } = new();
}

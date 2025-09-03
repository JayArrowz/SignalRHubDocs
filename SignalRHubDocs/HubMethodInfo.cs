using System.Collections.Generic;

namespace SignalRHubDocs;

public class HubMethodInfo
{
    public required string Name { get; set; }
    public required string Summary { get; set; }
    public required string Description { get; set; }
    public required string[] Tags { get; set; }
    public required string ReturnType { get; set; }
    public List<HubParameterInfo> Parameters { get; set; } = new();
    public bool RequiresAuth { get; set; }
    public string[]? RequiredRoles { get; set; }
    public string[]? RequiredPolicies { get; set; }
    public object? ReturnSchema { get; set; }
}

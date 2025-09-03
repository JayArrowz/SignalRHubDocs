namespace SignalRHubDocs;

public class HubParameterInfo
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public bool IsOptional { get; set; }
    public object? DefaultValue { get; set; }
    public required string Description { get; set; }
    public object? Schema { get; set; }
    public bool SendEnumAsString { get; set; } = false;
}

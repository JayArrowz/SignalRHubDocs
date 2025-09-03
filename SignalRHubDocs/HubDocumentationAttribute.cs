using System;

namespace SignalRHubDocs;

[AttributeUsage(AttributeTargets.Class)]
public class HubDocumentationAttribute : Attribute
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public HubDocumentationAttribute(string? name = null, string? description = null)
    {
        Name = name;
        Description = description;
    }
}

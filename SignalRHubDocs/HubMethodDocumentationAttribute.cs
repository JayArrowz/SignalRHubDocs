using System;

namespace SignalRHubDocs;

[AttributeUsage(AttributeTargets.Method)]
public class HubMethodDocumentationAttribute : Attribute
{
    public string? Summary { get; set; }
    public string? Description { get; set; }
    public string[] Tags { get; set; }

    public HubMethodDocumentationAttribute(string? summary = null, string? description = null, params string[] tags)
    {
        Summary = summary;
        Description = description;
        Tags = tags ?? new string[0];
    }
}

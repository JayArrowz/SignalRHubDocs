using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace SignalRHubDocs;

public class SignalRDocumentationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HubInspectorService _inspector;
    private readonly EmbeddedTemplateService _templateService;
    private readonly SignalRDocumentationOptions _options;
    private readonly Assembly _entryAssembly;
    private string? _documentationJson;

    public SignalRDocumentationMiddleware(
        RequestDelegate next,
        HubInspectorService inspector,
        EmbeddedTemplateService templateService,
        SignalRDocumentationOptions options)
    {
        _next = next;
        _inspector = inspector;
        _templateService = templateService;
        _options = options;
        _entryAssembly = Assembly.GetEntryAssembly()!;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments(_options.RoutePrefix))
        {
            await HandleDocumentationRequest(context);
            return;
        }

        await _next(context);
    }

    private async Task HandleDocumentationRequest(HttpContext context)
    {
        var path = context.Request.Path.Value;

        if (path.EndsWith("/swagger.json") || path.EndsWith("/api.json"))
        {
            await ServeApiJson(context);
        }
        else
        {
            await ServeTestPage(context);
        }
    }

    private async Task ServeApiJson(HttpContext context)
    {
        var documentation = _inspector.GenerateDocumentation(_entryAssembly, _options.SendEnumByString);
        documentation.SupportedAuthSchemes = _options.SupportedAuthSchemes;
        documentation.Description = _options.Description;
        documentation.Title = _options.Title;
        documentation.Version = _options.Version;

        _documentationJson = _documentationJson ?? JsonSerializer.Serialize(documentation, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(_documentationJson);
    }

    private async Task ServeTestPage(HttpContext context)
    {
        var template = await _templateService.LoadTemplateAsync("testing");
        var html = _templateService.ProcessTemplate(template, new Dictionary<string, string>
        {
            { "Title", $"{_options.Title} - Testing Interface" },
            { "ApiJsonUrl", $"{_options.RoutePrefix}/api.json" },
            { "Description", _options.Description ?? "Test your SignalR hubs in real-time with support for multiple protocols, multiple simultaneous hub connections, and authentication." },
            { "SignalRClientUrl", _options.SignalRClientUrl },
            { "MessagePackClientUrl", _options.MessagePackClientUrl }
        });

        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync(html);
    }
}

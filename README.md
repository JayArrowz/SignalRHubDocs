# SignalRHubDocs

[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/SignalRHubDocs.svg)](https://www.nuget.org/packages/SignalRHubDocs)
[![Buy Me A Coffee](https://img.shields.io/badge/Buy%20Me%20A%20Coffee-support-yellow.svg)](https://buymeacoffee.com/jayarrowz)

Auto-generated documentation and testing interface for ASP.NET Core SignalR hubs, similar to Swagger for Web APIs.

## Features

- **Automatic Hub Discovery**: Scans your application for SignalR hubs and generates comprehensive documentation
- **Interactive Testing Interface**: Test hub methods directly in the browser with real-time connection management
- **Multiple Protocol Support**: JSON and MessagePack protocols
- **Authentication Support**: Bearer tokens, API keys, query parameters, cookies, and custom headers
- **Real-time Logging**: Live message monitoring across all connected hubs
- **Schema Generation**: Automatic parameter and return type schemas with examples
- **Streaming Support**: Full support for server-to-client streaming methods
- **Multiple Hub Connections**: Connect to multiple hubs simultaneously

## Installation

Install the NuGet package:

```bash
dotnet add package SignalRHubDocs
```

## Quick Start

### 1. Configure Services

Add SignalR documentation to your `Program.cs` or `Startup.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSignalRDocumentation(options =>
{
    options.Title = "My SignalR API";
    options.Description = "Real-time communication endpoints";
    options.Version = "1.0.0";
    options.RoutePrefix = "/signalr-docs"; // Default route
});

var app = builder.Build();

// Configure hubs
app.MapHub<ChatHub>("/chatHub");
app.MapHub<NotificationHub>("/notificationHub");

// Add documentation middleware
app.UseSignalRDocumentation();

app.Run();
```

### 2. Document Your Hubs

Use attributes to provide rich documentation:

```csharp
[HubDocumentation("Chat Hub", "Real-time messaging functionality")]
public class ChatHub : Hub
{
    [HubMethodDocumentation(
        summary: "Send message to all users",
        description: "Broadcasts a message to all connected clients",
        tags: new[] { "messaging", "broadcast" }
    )]
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    [HubMethodDocumentation(
        summary: "Join chat room",
        description: "Adds the user to a specific chat room group"
    )]
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("UserJoined", Context.User.Identity.Name);
    }

    [HubMethodDocumentation(
        summary: "Stream live updates",
        description: "Provides real-time streaming of data updates"
    )]
    public async IAsyncEnumerable<string> StreamUpdates(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            yield return $"Update at {DateTime.Now}";
            await Task.Delay(1000, cancellationToken);
        }
    }
}
```

### 3. Access Documentation

Navigate to `/signalr-docs` in your browser to access the interactive documentation and testing interface.

## Configuration Options

```csharp
builder.Services.AddSignalRDocumentation(options =>
{
    // Basic information
    options.Title = "SignalR API Documentation";
    options.Description = "Auto-generated hub documentation";
    options.Version = "1.0.0";
    options.RoutePrefix = "/signalr-docs";
    
    // Enum serialization
    options.SendEnumByString = false; // Send enums as numbers (default) or strings
    
    // Authentication schemes
    options.SupportedAuthSchemes.Add(new AuthScheme
    {
        Name = "Bearer",
        Type = AuthType.Bearer,
        Description = "JWT Bearer token authentication",
        IsDefault = true
    });
    
    options.SupportedAuthSchemes.Add(new AuthScheme
    {
        Name = "ApiKey",
        Type = AuthType.CustomHeader,
        HeaderName = "X-API-Key",
        Description = "API key authentication"
    });
});
```

## Authentication Support

SignalRHubDocs supports multiple authentication schemes:

### Bearer Token Authentication
```csharp
options.SupportedAuthSchemes.Add(new AuthScheme
{
    Name = "Bearer",
    Type = AuthType.Bearer,
    Description = "JWT Bearer token"
});
```

### API Key Authentication
```csharp
options.SupportedAuthSchemes.Add(new AuthScheme
{
    Name = "ApiKey",
    Type = AuthType.CustomHeader,
    HeaderName = "X-API-Key",
    Description = "API key in custom header"
});
```

### Query Parameter Authentication
```csharp
options.SupportedAuthSchemes.Add(new AuthScheme
{
    Name = "AccessToken",
    Type = AuthType.QueryParam,
    QueryParamName = "access_token",
    Description = "Token via query parameter"
});
```

## Hub Method Documentation

Use the `HubMethodDocumentationAttribute` to provide detailed method information:

```csharp
[HubMethodDocumentation(
    summary: "Brief method description",
    description: "Detailed explanation of what the method does",
    tags: new[] { "category1", "category2" }
)]
public async Task MyHubMethod(string param1, int param2)
{
    // Method implementation
}
```

## Authorization

The library automatically detects `[Authorize]` attributes and reflects authorization requirements in the documentation:

```csharp
[Authorize] // Hub-level authorization
public class SecureHub : Hub
{
    [Authorize(Roles = "Admin")] // Method-level authorization
    public async Task AdminOnlyMethod()
    {
        // Implementation
    }
}
```

## Advanced Features

### Complex Parameter Types

The library automatically generates schemas for complex types:

```csharp
public class ChatMessage
{
    public string User { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
}

public async Task SendComplexMessage(ChatMessage message)
{
    await Clients.All.SendAsync("ReceiveComplexMessage", message);
}
```

### Streaming Methods

Server-to-client streaming methods are automatically detected and documented:

```csharp
public async IAsyncEnumerable<WeatherData> GetWeatherStream(
    string location,
    [EnumeratorCancellation] CancellationToken cancellationToken)
{
    while (!cancellationToken.IsCancellationRequested)
    {
        yield return await GetWeatherData(location);
        await Task.Delay(5000, cancellationToken);
    }
}
```

### Enum Support

Enums are automatically documented with their values:

```csharp
public enum MessageType
{
    Info = 0,
    Warning = 1,
    Error = 2
}

public async Task SendTypedMessage(string content, MessageType type)
{
    await Clients.All.SendAsync("ReceiveTypedMessage", content, type);
}
```

## API Endpoints

- `GET /signalr-docs` - Interactive documentation and testing interface
- `GET /signalr-docs/api.json` - JSON schema of all documented hubs
- `GET /signalr-docs/swagger.json` - Alias for api.json

## Multi-Framework Support

SignalRHubDocs targets multiple .NET versions for broad compatibility:

```xml
<TargetFrameworks>net9.0;net8.0;net7.0;net6.0;net5.0</TargetFrameworks>
```

The library automatically works with all supported frameworks, and you can configure client library versions to match your deployment environment or security requirements.

The testing interface supports modern browsers with WebSocket capability. The generated documentation works with:

- Chrome 16+
- Firefox 11+
- Safari 7+
- Edge (all versions)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## ☕ Support

If you find CompactPack useful and want to support its development:

[![Buy Me A Coffee](https://img.shields.io/badge/Buy%20Me%20A%20Coffee-support-yellow.svg)](https://buymeacoffee.com/jayarrowz)

## License

This project is licensed under the MIT License - see the LICENSE file for details.
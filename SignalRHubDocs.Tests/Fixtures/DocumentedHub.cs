using Microsoft.AspNetCore.SignalR;

namespace SignalRHubDocs.Tests;

[HubDocumentation("Custom Hub Name", "Custom hub description")]
public class DocumentedHub : Hub
{
    public async Task DocumentedMethod()
    {
        await Clients.All.SendAsync("Test");
    }
}

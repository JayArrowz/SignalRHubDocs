using Microsoft.AspNetCore.SignalR;

namespace SignalRHubDocs.Tests;

public class HubWithFrameworkMethods : Hub
{
    public async Task UserMethod()
    {
        await Clients.All.SendAsync("User");
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}

using Microsoft.AspNetCore.SignalR;

namespace SignalRHubDocs.Tests;

[Microsoft.AspNetCore.Authorization.Authorize]
public class AuthorizedIntegrationHub : Hub
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public async Task AdminAction()
    {
        await Clients.All.SendAsync("AdminMessage");
    }
}

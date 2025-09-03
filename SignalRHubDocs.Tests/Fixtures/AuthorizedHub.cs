using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SignalRHubDocs.Tests;

[Authorize]
public class AuthorizedHub : Hub
{
    public async Task SecureMethod()
    {
        await Clients.All.SendAsync("Secure");
    }

    [Authorize(Roles = "Admin")]
    public async Task AdminMethod()
    {
        await Clients.All.SendAsync("Admin");
    }
}

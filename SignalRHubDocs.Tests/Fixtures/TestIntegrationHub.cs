using Microsoft.AspNetCore.SignalR;

namespace SignalRHubDocs.Tests;

public class TestIntegrationHub : Hub
{
    [HubMethodDocumentation("Send a message", "Sends a message to all clients", "messaging", "broadcast")]
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task<string> GetConnectionId()
    {
        return Context.ConnectionId;
    }

    public async IAsyncEnumerable<int> StreamNumbers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return i;
            await Task.Delay(100);
        }
    }
}


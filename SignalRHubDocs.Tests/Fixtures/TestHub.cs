using Microsoft.AspNetCore.SignalR;

namespace SignalRHubDocs.Tests;

public class TestHub : Hub
{
    public async Task TestMethod()
    {
        await Clients.All.SendAsync("Test");
    }

    public async Task MethodWithParameters(string message, int count)
    {
        await Clients.All.SendAsync("Message", message, count);
    }

    public async IAsyncEnumerable<string> StreamingMethod()
    {
        yield return "test";
    }
}

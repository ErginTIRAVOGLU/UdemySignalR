using Microsoft.AspNetCore.SignalR;

namespace UdemySignalR.API.Hubs;

public class MyHub:Hub
{
    private static List<string> Names { get; set; } = new List<string>();
    private static List<string> ClientList { get; set; } = new List<string>();
    private static int ClientCount { get; set; } = 0;

    
    public async Task SendName(string name)
    {
        Names.Add(name);
        await Clients.All.SendAsync("ReceiveName", name);
    }

    public async Task GetNames()
    {
        await Clients.All.SendAsync("ReceiveNames", Names);
    }

    public async override Task OnConnectedAsync()
    {
        ClientCount++;
        await Clients.All.SendAsync("ReceiveClientCount", ClientCount);
        await base.OnConnectedAsync();
    }
    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        ClientCount--;
        await Clients.All.SendAsync("ReceiveClientCount", ClientCount);
        await base.OnDisconnectedAsync(exception);
    }
}

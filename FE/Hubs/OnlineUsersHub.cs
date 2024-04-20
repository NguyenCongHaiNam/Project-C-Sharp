using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class OnlineUsersHub : Hub
{
    private static int onlineUsersCount = 0;

    public override async Task OnConnectedAsync()
    {
        onlineUsersCount++;
        await Clients.All.SendAsync("UpdateOnlineUsersCount", onlineUsersCount);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        onlineUsersCount--;
        await Clients.All.SendAsync("UpdateOnlineUsersCount", onlineUsersCount);
        await base.OnDisconnectedAsync(exception);
    }
}

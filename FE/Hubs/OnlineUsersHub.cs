using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class OnlineUsersHub : Hub
{
    private static int onlineUsersCount = 0;

    public async Task UserLoggedIn()
    {
        onlineUsersCount++;
        await Clients.All.SendAsync("UpdateOnlineUsersCount", onlineUsersCount);
    }

    public async Task UserLoggedOut()
    {
        onlineUsersCount--;
        await Clients.All.SendAsync("UpdateOnlineUsersCount", onlineUsersCount);
    }
}


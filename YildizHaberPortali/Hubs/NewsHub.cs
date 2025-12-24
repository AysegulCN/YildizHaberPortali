using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace YildizHaberPortali.Hubs
{
    public class NewsHub : Hub
    {
        public async Task SendNotification(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", user, message);
        }
    }
}
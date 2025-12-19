using Microsoft.AspNetCore.SignalR;

namespace YildizHaberPortali.Hubs
{
    public class NewsHub : Hub
    {
        public async Task SendCommentNotification(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", user, message);
        }
    }
}
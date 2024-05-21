using Microsoft.AspNetCore.SignalR;

namespace Web_API.Hubs
{
    public class Chat : Hub<IChat>
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage( user, message);
        }
    }
} 
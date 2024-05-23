namespace Web_API.Hubs;

public interface IChat
{
    Task ReceiveMessage(string user, string message);

    Task ReceiveInvitation(string roomName);
}
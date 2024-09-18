namespace MetroVoip.Business.Interfaces
{
    public interface INotificationHub
    {
        Task OnConnectedAsync();
        Task OnDisconnectedAsync(Exception exception);
        Task SendVoiceRequest(string listenerConnectionId);
        Task AcceptVoiceRequest(string listenerConnectionId);
    }
}

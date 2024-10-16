namespace MetroVoip.Business.Interfaces
{
    public interface ICommendIntercomService
    {
        Task AcceptIncomingCall(string ipAddress, string username, string password);
        Task CancelCall(string ipAddress, string username, string password);
        Task DeclineCall(string ipAddress, string username, string password);



        // Header kullanarak gelen aramayı kabul et
        Task<bool> AcceptIncomingCallAsync(string ipAddress, string username, string password);
    }
}

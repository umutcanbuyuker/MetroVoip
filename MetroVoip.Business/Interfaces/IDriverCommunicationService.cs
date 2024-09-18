namespace MetroVoip.Business.Interfaces
{
    public interface IDriverCommunicationService
    {
        Task StartSpeakingWithPassenger(int kabinId);
        Task StopSpeakingWithPassenger(int kabinId);
        Task StartListeningPassenger(int kabinId);
    }
}

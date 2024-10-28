namespace MetroVoip.Business.Interfaces
{
    public interface IDriverCommunicationService
    {
        Task StartSpeakingWithPassenger();
        Task StopSpeakingWithPassenger(int kabinId);
        Task StartListeningPassenger(int kabinId);
        Task StartSipCall();
        void EndSipCall();
    }
}

namespace MetroVoip.Business.Interfaces
{
    public interface IPassengerCommunicationService
    {
        Task StartSpeakingWithDriver();
        Task StopSpeakingWithDriver();
        Task StartListeningDriver();

    }
}

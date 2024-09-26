namespace MetroVoip.Business.Interfaces
{
    public interface IPassengerCommunicationService
    {
        void StartSpeakingWithDriver(string ip, int rPort, int sPort);
        void StopSpeakingWithDriver(string ip);
        void StartListeningWithDriver(string ip, int rPort, int sPort);
        void StopListeningWithDriver(string ip);
    }
}

namespace MetroVoip.Business.Interfaces
{
    public interface IDriverCommunicationService
    {
        void StartSpeakingWithPassenger(string ip, int rPort, int sPort);
        void StopSpeakingWithPassenger(string ip);
        void StartListeningWithPassenger(string ip, int rPort, int sPort);
        void StopListeningWithPassenger(string ip);

    }
}

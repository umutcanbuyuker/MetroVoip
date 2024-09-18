namespace MetroVoip.Business.Interfaces
{
    public interface ICommunicationService
    {
        void StartSpeaking(string ip, int rPort, int sPort);
        void StopSpeaking(string ip);
    }
}

namespace MetroVoip.Business.Interfaces
{
    public interface ICommunicationService
    {
        void StartSpeaking(int kabinId);
        void StopSpeaking(int kabinId);
    }
}

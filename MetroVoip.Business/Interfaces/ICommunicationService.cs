namespace MetroVoip.Business.Interfaces
{
    public interface ICommunicationService
    {
        Task StartSpeaking(int kabinId);
        Task StopSpeaking(int kabinId);
    }
}

namespace MetroVoip.Business.Interfaces
{
    public interface ITelesteAresService
    {
        Task StartStreamingAsync(string ipAddress, int port);
        Task StopStreamingAsync(string ipAddress, int port);
        Task SwitchToTcpModeAsync(string ipAddress, int port);
        Task SwitchToUdpModeAsync(string ipAddress, int port);
        Task SwitchToMulticastModeAsync(string multicastAddress, int port);
        Task SendAudioPacketAsync(string ipAddress, int port, byte[] audioData);
        Task ReceiveAudioPacketAsync(string ipAddress, int port);
        Task SetVolumeAsync(string ipAddress, int volumeLevel);
    }
}

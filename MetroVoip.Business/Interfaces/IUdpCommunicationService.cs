using MetroVoip.Common.DTOs;

namespace MetroVoip.Business.Interfaces
{
    public interface IUdpCommunicationService
    {
        Task StartListeningAsync(int carriageNumber);
        Task StopListeningAsync(int carriageNumber);
        Task SendDataAsync(UdpCommunicationDto dto);
    }
}

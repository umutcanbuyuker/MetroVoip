using MetroVoip.Common.Models;

namespace MetroVoip.Business.Interfaces
{
    public interface IIntercomService
    {
        Task StartCommunicationAsync(int carriageNumber);
        Task StopCommunicationAsync(int carriageNumber);
        Task<List<IntercomModel>> GetAllIntercomsAsync();
    }
}

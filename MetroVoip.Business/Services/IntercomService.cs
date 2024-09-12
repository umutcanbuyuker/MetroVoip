using MetroVoip.Business.Interfaces;
using MetroVoip.Common.Models;

namespace MetroVoip.Business.Services
{
    public class IntercomService : IIntercomService
    {
        private readonly IUdpCommunicationService _udpService;
        public IntercomService(IUdpCommunicationService udpService)
        {
            _udpService = udpService;
        }
        public async Task StartCommunicationAsync(int carriageNumber)
        {
            await _udpService.StartListeningAsync(carriageNumber);
        }

        public async Task StopCommunicationAsync(int carriageNumber)
        {
            await _udpService.StopListeningAsync(carriageNumber);
        }
        public async Task<List<IntercomModel>> GetAllIntercomsAsync()
        {
            return await Task.FromResult(new List<IntercomModel>
        {
            new IntercomModel { CarriageNumber = 1, IsCommunicationActive = false },
            new IntercomModel { CarriageNumber = 2, IsCommunicationActive = false },
            new IntercomModel { CarriageNumber = 3, IsCommunicationActive = false },
            new IntercomModel { CarriageNumber = 4, IsCommunicationActive = false }
        });
        }
    }
}

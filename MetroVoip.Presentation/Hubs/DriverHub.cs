using MetroVoip.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MetroVoip.Presentation.Hubs
{
    public class DriverHub : Hub
    {
        public DriverHub()
        {
           
        }
        public async Task SendCabinRequestAsync(int cabinId)
        {
            await Clients.All.SendAsync("ReceiveCabinRequest", cabinId);
        }
        public async Task SendReleaseCabinRequestAsync(int cabinId)
        {
            await Clients.All.SendAsync("ReceiveCabinRelease", cabinId);
        }
        public async Task SendDriverRequestAsync(int cabinId)
        {
            await Clients.All.SendAsync("ReceiveDriverRequest", cabinId);
        }
    }
}

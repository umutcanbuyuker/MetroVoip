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


        ///WebRtc
        public async Task SendSignal(string user, string signal)
        {
            await Clients.Others.SendAsync("ReceiveSignal", Context.ConnectionId, signal);
        }

        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("YourConnectionId", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        //ConferenceHub
        private static Dictionary<string, (string PeerId, string UserName)> ConnectedPeers = new Dictionary<string, (string, string)>();

        public async Task RegisterPeerWithName(string peerId, string userName)
        {
            if (!ConnectedPeers.ContainsKey(Context.ConnectionId))
            {
                ConnectedPeers[Context.ConnectionId] = (peerId, userName);
            }
            await Clients.All.SendAsync("UpdatePeerList", ConnectedPeers.Values.Select(p => new { id = p.PeerId, name = p.UserName }));
        }
        public async Task AcceptCall(string callerConnectionId)
        {
            await Clients.Client(callerConnectionId).SendAsync("CallAccepted");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (ConnectedPeers.ContainsKey(Context.ConnectionId))
            {
                ConnectedPeers.Remove(Context.ConnectionId);
            }
            await Clients.All.SendAsync("UpdatePeerList", ConnectedPeers.Values.Select(p => new { id = p.PeerId, name = p.UserName }));
            await base.OnDisconnectedAsync(exception);
        }
    }
}

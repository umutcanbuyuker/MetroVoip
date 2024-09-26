using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;

public class VoiceChatHub : Hub
{
    private static Dictionary<string, string> ConnectedPeers = new Dictionary<string, string>();

    public async Task RegisterPeer(string peerId)
    {
        // Bağlantı kimliği ile peer ID'yi ilişkilendir
        if (!ConnectedPeers.ContainsKey(Context.ConnectionId))
        {
            ConnectedPeers[Context.ConnectionId] = peerId;
        }

        await Clients.All.SendAsync("UpdatePeerList", ConnectedPeers.Values);
    }

    public async Task RequestCall(string targetPeerId)
    {
        // Aranan kullanıcıyı bul ve ona çağrı isteği gönder
        var targetConnectionId = ConnectedPeers.FirstOrDefault(x => x.Value == targetPeerId).Key;
        if (targetConnectionId != null)
        {
            await Clients.Client(targetConnectionId).SendAsync("ReceiveCallRequest", Context.ConnectionId);
        }
    }

    public async Task AcceptCall(string callerConnectionId)
    {
        await Clients.Client(callerConnectionId).SendAsync("CallAccepted");
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // Kullanıcı bağlantısı kesildiğinde, onu listeden çıkar
        if (ConnectedPeers.ContainsKey(Context.ConnectionId))
        {
            ConnectedPeers.Remove(Context.ConnectionId);
        }
        await Clients.All.SendAsync("UpdatePeerList", ConnectedPeers.Values);
        await base.OnDisconnectedAsync(exception);
    }
}

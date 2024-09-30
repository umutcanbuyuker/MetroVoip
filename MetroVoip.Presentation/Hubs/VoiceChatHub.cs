using Microsoft.AspNetCore.SignalR;

namespace MetroVoip.Presentation.Hubs
{
    public class VoiceChatHub : Hub
    {
        private static Dictionary<string, string> ConnectedPeers = new Dictionary<string, string>();
        private static string AdminConnectionId = null;

        public async Task RegisterPeer(string peerId)
        {
            // Bağlantı kimliği ile peer ID'yi ilişkilendir
            if (!ConnectedPeers.ContainsKey(Context.ConnectionId))
            {
                ConnectedPeers[Context.ConnectionId] = peerId;
            }

            // Bağlanan kullanıcıların listesini tüm kullanıcılara gönder
            await Clients.All.SendAsync("UpdatePeerList", ConnectedPeers.Values);
        }

        public async Task RegisterAdmin()
        {
            AdminConnectionId = Context.ConnectionId;

            // Admin'e tüm kullanıcıların listesini gönder
            await Clients.Client(AdminConnectionId).SendAsync("UpdatePeerList", ConnectedPeers.Values);
        }

        public async Task StartVoiceCommunication(List<string> selectedPeers)
        {
            // Yeni bir grup oluştur
            string groupName = $"Group_{Guid.NewGuid()}";

            // Seçilen her konuşmacıyı gruba ekle
            foreach (var peerId in selectedPeers)
            {
                var connectionId = ConnectedPeers.FirstOrDefault(x => x.Value == peerId).Key;
                if (!string.IsNullOrEmpty(connectionId))
                {
                    await Groups.AddToGroupAsync(connectionId, groupName);
                }
            }

            // Grup içindeki konuşmacılara sesli iletişim başlatmalarını söylüyoruz
            await Clients.Group(groupName).SendAsync("StartVoiceCall", selectedPeers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (ConnectedPeers.ContainsKey(Context.ConnectionId))
            {
                ConnectedPeers.Remove(Context.ConnectionId);
            }

            // Admin'e güncellenmiş Peer listesi gönder
            if (AdminConnectionId != null && AdminConnectionId == Context.ConnectionId)
            {
                AdminConnectionId = null;
            }

            await Clients.All.SendAsync("UpdatePeerList", ConnectedPeers.Values);
            await base.OnDisconnectedAsync(exception);
        }
    }
}

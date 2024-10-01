using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

public class VoiceChatHub : Hub
{
    private static Dictionary<string, string> ConnectedPeers = new Dictionary<string, string>();
    private static string AdminConnectionId = null;

    // Aktif grupları saklamak için yeni bir sözlük
    private static Dictionary<string, List<string>> ActiveGroups = new Dictionary<string, List<string>>();

    public async Task RegisterPeer(string peerId)
    {
        // Bağlantı kimliği ile peer ID'yi ilişkilendir
        if (!ConnectedPeers.ContainsKey(Context.ConnectionId))
        {
            ConnectedPeers[Context.ConnectionId] = peerId;
            Console.WriteLine($"Peer kayıt oldu: {peerId} (Connection ID: {Context.ConnectionId})");
        }

        // Bağlanan kullanıcıların listesini tüm kullanıcılara gönder (grupta olmayanlar)
        var availablePeers = GetAvailablePeers();
        await Clients.All.SendAsync("UpdatePeerList", availablePeers);
    }

    public async Task RegisterAdmin()
    {
        AdminConnectionId = Context.ConnectionId;
        Console.WriteLine($"Admin kayıt oldu: Connection ID: {AdminConnectionId}");

        // Admin'e tüm kullanıcıların listesini gönder (grupta olmayanlar)
        var availablePeers = GetAvailablePeers();
        await Clients.Client(AdminConnectionId).SendAsync("UpdatePeerList", availablePeers);

        // Admin'e mevcut aktif grupların listesini gönder
        await Clients.Client(AdminConnectionId).SendAsync("UpdateActiveGroups", ActiveGroups.Keys.ToList());
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
                Console.WriteLine($"Peer {peerId} gruba eklendi: {groupName}");
            }
        }

        // Aktif gruplara ekle
        ActiveGroups[groupName] = selectedPeers;
        Console.WriteLine($"Yeni grup oluşturuldu: {groupName} - Üyeler: {string.Join(", ", selectedPeers)}");

        // Yöneticiye güncellenmiş aktif grup listesi gönder
        if (AdminConnectionId != null)
        {
            await Clients.Client(AdminConnectionId).SendAsync("UpdateActiveGroups", ActiveGroups.Keys.ToList());
            Console.WriteLine("Yöneticiye güncellenmiş aktif gruplar gönderildi.");
        }

        // Grup içindeki konuşmacılara sesli iletişim başlatmalarını söylüyoruz
        await Clients.Group(groupName).SendAsync("StartVoiceCall", selectedPeers, groupName);
        Console.WriteLine($"StartVoiceCall mesajı gönderildi: {groupName}");

        // Bağlanan kullanıcılar listesini güncelle
        var availablePeers = GetAvailablePeers();
        await Clients.All.SendAsync("UpdatePeerList", availablePeers);
    }

    // Sesli iletişimi sonlandırma metodu
    public async Task EndVoiceCommunication(string groupName)
    {
        Console.WriteLine($"EndVoiceCommunication çağrıldı: {groupName}");

        if (ActiveGroups.ContainsKey(groupName))
        {
            var peers = ActiveGroups[groupName];

            // Öncelikle gruptaki konuşmacılara sonlandırma mesajını gönder
            await Clients.Group(groupName).SendAsync("EndVoiceCall", groupName);
            Console.WriteLine($"EndVoiceCall mesajı gönderildi: {groupName}");

            // Daha sonra gruptan üyeleri çıkar
            foreach (var peerId in peers)
            {
                var connectionId = ConnectedPeers.FirstOrDefault(x => x.Value == peerId).Key;
                if (!string.IsNullOrEmpty(connectionId))
                {
                    await Groups.RemoveFromGroupAsync(connectionId, groupName);
                    Console.WriteLine($"Peer {peerId} gruptan çıkarıldı: {groupName}");
                }
            }

            // Grup sonlandırıldıktan sonra grubu aktif gruplardan çıkar
            ActiveGroups.Remove(groupName);
            Console.WriteLine($"Grup aktif gruplardan çıkarıldı: {groupName}");

            // Yöneticiye güncellenmiş aktif grup listesi gönder
            if (AdminConnectionId != null)
            {
                await Clients.Client(AdminConnectionId).SendAsync("UpdateActiveGroups", ActiveGroups.Keys.ToList());
                Console.WriteLine("Yöneticiye güncellenmiş aktif gruplar gönderildi.");
            }

            // Bağlanan kullanıcılar listesini güncelle
            var availablePeers = GetAvailablePeers();
            await Clients.All.SendAsync("UpdatePeerList", availablePeers);
        }
        else
        {
            Console.WriteLine($"EndVoiceCommunication: Grup bulunamadı: {groupName}");
        }
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (ConnectedPeers.ContainsKey(Context.ConnectionId))
        {
            string disconnectedPeerId = ConnectedPeers[Context.ConnectionId];
            ConnectedPeers.Remove(Context.ConnectionId);
            Console.WriteLine($"Peer bağlantısı kesildi: {disconnectedPeerId}");

            // Aktif gruplardan bu kullanıcıyı çıkar
            var groupsToUpdate = ActiveGroups.Where(g => g.Value.Contains(disconnectedPeerId)).Select(g => g.Key).ToList();
            foreach (var group in groupsToUpdate)
            {
                ActiveGroups[group].Remove(disconnectedPeerId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
                Console.WriteLine($"Peer {disconnectedPeerId} gruptan çıkarıldı: {group}");

                // Grup boş kaldıysa grubu sonlandır
                if (ActiveGroups[group].Count < 2)
                {
                    await EndVoiceCommunication(group);
                }
                else
                {
                    // Grup hala aktif ise, grubu güncelle
                    if (AdminConnectionId != null)
                    {
                        await Clients.Client(AdminConnectionId).SendAsync("UpdateActiveGroups", ActiveGroups.Keys.ToList());
                        Console.WriteLine("Yöneticiye güncellenmiş aktif gruplar gönderildi.");
                    }
                }
            }

            // Admin bağlantısını kontrol et
            if (AdminConnectionId != null && AdminConnectionId == Context.ConnectionId)
            {
                AdminConnectionId = null;
                Console.WriteLine("Admin bağlantısı kesildi.");
            }

            // Tüm kullanıcılara güncellenmiş Peer listesini gönder (grupta olmayanlar)
            var availablePeers = GetAvailablePeers();
            await Clients.All.SendAsync("UpdatePeerList", availablePeers);
            Console.WriteLine("Tüm kullanıcılara güncellenmiş Peer listesi gönderildi.");
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Yardımcı Metot: Grup konuşmasına dahil olmayan kullanıcıları döndürür
    private List<string> GetAvailablePeers()
    {
        var groupedPeers = ActiveGroups.Values.SelectMany(g => g).Distinct();
        var availablePeers = ConnectedPeers.Values.Where(peer => !groupedPeers.Contains(peer)).ToList();
        return availablePeers;
    }
}

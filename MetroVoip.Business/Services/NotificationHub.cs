// Hubs/NotificationHub.cs
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;


// Dinleyici bilgileri için bir model sınıfı
public class ListenerInfo
{
    public string ConnectionId { get; set; }
    public string RequestIpAddress { get; set; }
    public string IpAddress { get; set; }
    public int SpeakPort { get; set; }
    public int ListenPort { get; set; }
    public bool HasActiveRequest { get; set; } // Yeni bayrak
}

// Yönetici bilgileri için bir model sınıfı
public class ManagerInfo
{
    public string IpAddress { get; set; }
    public int SpeakPort { get; set; }
    public int ListenPort { get; set; }

}

public class NotificationHub : Hub
{
    // Dinleyici bilgilerini saklamak için statik bir ConcurrentDictionary
    private static ConcurrentDictionary<string, ListenerInfo> Listeners = new();

    private readonly ManagerInfo managerInfo = new() {
        IpAddress = "10.2.20.27",
        ListenPort = 5000,
        SpeakPort = 5000
    };



    // Bağlanıldığında tüm dinleyicileri gönder.
    public override async Task OnConnectedAsync()
    {
        // Yeni bağlanan yöneticiye mevcut dinleyici listesini gönder
        await Clients.Caller.SendAsync("RegisterListener", Listeners.Values);
        foreach (var listenerInfo in Listeners.Values)
        {
            // Eğer dinleyicinin aktif bir isteği varsa yeni bir istek gönderemez
            if (listenerInfo.HasActiveRequest)
            {
                await Clients.All.SendAsync("ReceiveNotification", listenerInfo);
            }
        }


        await base.OnConnectedAsync();
    }

    // Dinleyici bilgilerini hub'a gönderir ve yöneticinin ekranını günceller
    public async Task RegisterListener(string ipAddress, int speakPort, int listenPort)
    {
        // Dinleyici bilgilerini ekleme veya güncelleme
        Listeners[Context.ConnectionId] = new ListenerInfo
        {
            ConnectionId = Context.ConnectionId.ToString(),
            IpAddress = ipAddress,
            SpeakPort = speakPort,
            ListenPort = listenPort
        };

        // Yöneticinin ekranına güncellenmiş dinleyici listesi gönder
        await Clients.All.SendAsync("RegisterListener", Listeners.Values);


    }

    public async Task StartListening()
    {
        await Clients.All.SendAsync("StartListening", managerInfo);
    }
    public async Task StopListening()
    {
        await Clients.All.SendAsync("StopListening", true);
    }

    public async Task StartSpeaking()
    {
        await Clients.All.SendAsync("StartSpeaking", managerInfo);
    }
    public async Task StopSpeaking()
    {
        await Clients.All.SendAsync("StopSpeaking", true);
    }


    // Dinleyici butona bastığında tetiklenen method
    public async Task SendNotification()
    {
        // Dinleyici mevcutsa (bağlantı kontrolü yapıyoruz)
        if (Listeners.TryGetValue(Context.ConnectionId, out var listenerInfo))
        {
            // Eğer dinleyicinin aktif bir isteği varsa yeni bir istek gönderemez
            if (listenerInfo.HasActiveRequest)
            {
                // İkinci istek yapılmaya çalışıldığında bir mesaj dönebilirsiniz
                //await Clients.Caller.SendAsync("RequestDenied", "You already have an active notification.");
                return;
            }

            // İsteği işaretle
            listenerInfo.HasActiveRequest = true;

            // Yöneticinin ekranına "bildirim" bilgisini gönder
            await Clients.All.SendAsync("ReceiveNotification", listenerInfo);
        }
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // Bağlantı koptuğunda dinleyici listesinden çıkar
        if (Listeners.TryRemove(Context.ConnectionId, out var listenerInfo))
        {
            // Dinleyicinin aktif bildirim isteğini temizle
            //listenerInfo.HasActiveRequest = false;

            // Güncellenmiş dinleyici listesini yöneticinin ekranına gönder
            await Clients.All.SendAsync("RegisterListener", Listeners.Values);

            // Kapanan dinleyicinin bildirimlerini temizle
            // Yalnızca yöneticiyi bilgilendir
            await Clients.All.SendAsync("RemoveNotification", listenerInfo);

        }

        await base.OnDisconnectedAsync(exception);
    }

}
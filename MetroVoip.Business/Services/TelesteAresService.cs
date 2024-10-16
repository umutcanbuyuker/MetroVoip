using MetroVoip.Business.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace MetroVoip.Business.Services
{
    public class TelesteAresService : ITelesteAresService
    {
        private UdpClient _udpClient;
        private TcpClient _tcpClient;

        // UDP üzerinden ses akışı başlatma
        public async Task StartStreamingAsync(string ipAddress, int port)
        {
            _udpClient = new UdpClient();
            _udpClient.Connect(ipAddress, port); // UDP bağlantısını başlatır
            await Task.CompletedTask;
        }

        // TCP üzerinden bağlantı kurma
        public async Task SwitchToTcpModeAsync(string ipAddress, int port)
        {
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(ipAddress, port); // TCP bağlantısını başlatır
        }

        // UDP bağlantısına geçiş yapma
        public async Task SwitchToUdpModeAsync(string ipAddress, int port)
        {
            _udpClient = new UdpClient();
            _udpClient.Connect(ipAddress, port); // UDP ile bağlantı kurar
            await Task.CompletedTask;
        }

        // Multicast grubu üzerinden bağlantı kurma
        public async Task SwitchToMulticastModeAsync(string multicastAddress, int port)
        {
            _udpClient = new UdpClient();
            IPAddress multicastIP = IPAddress.Parse(multicastAddress);

            _udpClient.JoinMulticastGroup(multicastIP); // Multicast grubuna katılır
            _udpClient.Connect(multicastAddress, port); // Multicast ile bağlantı kurar
            await Task.CompletedTask;
        }

        // Ses verilerini UDP/TCP ile gönderme
        public async Task SendAudioPacketAsync(string ipAddress, int port, byte[] audioData)
        {
            if (_udpClient != null)
            {
                await _udpClient.SendAsync(audioData, audioData.Length); // UDP üzerinden ses gönderme
            }
            else if (_tcpClient != null)
            {
                NetworkStream stream = _tcpClient.GetStream();
                await stream.WriteAsync(audioData, 0, audioData.Length); // TCP üzerinden ses gönderme
            }
        }

        // Ses paketlerini UDP ile dinleme
        public async Task ReceiveAudioPacketAsync(string ipAddress, int port)
        {
            if (_udpClient != null)
            {
                UdpReceiveResult result = await _udpClient.ReceiveAsync(); // UDP üzerinden ses verilerini alma
                byte[] receivedData = result.Buffer;
                // Gelen ses verilerini burada işleyebilirsiniz
            }
        }

        // Ses seviyesi ayarlama (cihazın böyle bir API desteği varsa)
        public async Task SetVolumeAsync(string ipAddress, int volumeLevel)
        {
            // Ses seviyesi kontrolü için cihazın belirttiği API'yi kullanabilirsiniz
            await Task.CompletedTask;
        }

        // Streaming'i durdurma
        public async Task StopStreamingAsync(string ipAddress, int port)
        {
            if (_udpClient != null)
            {
                _udpClient.Close(); // UDP bağlantısını sonlandır
            }

            if (_tcpClient != null)
            {
                _tcpClient.Close(); // TCP bağlantısını sonlandır
            }

            await Task.CompletedTask;
        }
    }
}

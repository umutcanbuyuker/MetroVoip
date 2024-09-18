using MetroVoip.Business.Interfaces;
using NAudio.Wave;
using System.Net;
using System.Net.Sockets;

namespace MetroVoip.Business.Services
{
    public class DriverCommunicationService : IDriverCommunicationService
    {
        private UdpClient udpClient;
        private WaveInEvent waveIn;

        // Cabin's IPs
        private static Dictionary<int, string> kabinIPs = new Dictionary<int, string>
        {
            { 1, "10.2.149.24" },
            { 2, "10.2.20.27" },
            { 3, "127.0.0.2" },
            { 4, "127.0.0.3" }
        };

        public async Task StartSpeakingWithPassenger(int kabinId)
        {
            if (udpClient == null)
            {
                string serverIP = kabinIPs[kabinId];
                int serverPort = 9500;

                udpClient = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

                waveIn = new WaveInEvent
                {
                    WaveFormat = new WaveFormat(8000, 16, 1) // 8 kHz, 16 bit, mono
                };

                // Mikrofon verisini UDP üzerinden gönderme
                waveIn.DataAvailable += async (sender, e) =>
                {
                    await udpClient.SendAsync(e.Buffer, e.BytesRecorded, endPoint);
                    Console.WriteLine($"Kabin {kabinId} ile veri gönderildi: {e.BytesRecorded} byte");
                };

                waveIn.StartRecording();
            }
        }

        // Stop the communication with passenger.
        public async Task StopSpeakingWithPassenger(int kabinId)
        {
            if (waveIn != null)
            {
                waveIn.StopRecording();
                waveIn.Dispose();
                waveIn = null;
                Console.WriteLine($"Kabin {kabinId} ile konuşma durduruldu.");
            }

            if (udpClient != null)
            {
                udpClient.Dispose();
                udpClient = null;
                Console.WriteLine($"UDP bağlantısı kapatıldı.");
            }

            await Task.CompletedTask;
        }

        public async Task StartListeningPassenger(int kabinId)
        {
            int listenPort = 9000;

            using (UdpClient udpClient = new UdpClient(listenPort))
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
                Console.WriteLine($"UDP sunucusu {listenPort} portunda dinliyor...");

                // Ses verisini hoparlörde çalmak için WaveOut ve BufferedWaveProvider kullanıyoruz
                var waveOut = new WaveOutEvent();
                var bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(8000, 16, 1)); // 8kHz, 16bit, mono
                waveOut.Init(bufferedWaveProvider);
                waveOut.Play();

                while (true)
                {
                    // UDP paketini al
                    byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
                    Console.WriteLine($"Veri alındı: {receivedBytes.Length} byte");

                    // Alınan ses verisini hoparlörde oynat
                    bufferedWaveProvider.AddSamples(receivedBytes, 0, receivedBytes.Length);
                }
            }
        }
        
    }
}

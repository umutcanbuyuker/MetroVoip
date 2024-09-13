using MetroVoip.Business.Interfaces;
using NAudio.Wave;
using System.Net;
using System.Net.Sockets;

namespace MetroVoip.Business.Services
{
    public class CommunicationService : ICommunicationService
    {
        // Sınıf düzeyinde tanımlanan nesneler
        private UdpClient udpClient;
        private WaveInEvent waveIn;

        // Kabin IP'leri
        private static Dictionary<int, string> kabinIPs = new Dictionary<int, string>
    {
        { 1, "10.2.149.24" },
        { 2, "10.2.20.27" },
        { 3, "127.0.0.3" },
        { 4, "127.0.0.4" }
    };

        public void StartSpeaking(int kabinId)
        {
            if (udpClient == null)
            {
                string serverIP = kabinIPs[kabinId];
                int serverPort = 7000;

                udpClient = new UdpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

                waveIn = new WaveInEvent
                {
                    WaveFormat = new WaveFormat(8000, 16, 1) // 8 kHz, 16 bit, mono
                };

                // Mikrofon verisini UDP üzerinden gönderme
                waveIn.DataAvailable += (sender, e) =>
                {
                    udpClient.Send(e.Buffer, e.BytesRecorded, endPoint);
                    Console.WriteLine($"Kabin {kabinId} ile veri gönderildi: {e.BytesRecorded} byte");
                };

                waveIn.StartRecording();
            }
        }

        // Konuşmayı durdurma
        public void StopSpeaking(int kabinId)
        {
            // Eğer waveIn nesnesi null değilse kaydı durdur
            if (waveIn != null)
            {
                waveIn.StopRecording();
                waveIn.Dispose();
                waveIn = null;
                Console.WriteLine($"Kabin {kabinId} ile konuşma durduruldu.");
            }

            // Eğer udpClient null değilse kapat
            if (udpClient != null)
            {
                udpClient.Dispose();
                udpClient = null;
                Console.WriteLine($"UDP bağlantısı kapatıldı.");
            }
        }
    }
}

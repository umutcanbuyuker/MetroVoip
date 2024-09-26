using MetroVoip.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using NAudio.Wave;
using System.Net;
using System.Net.Sockets;

namespace MetroVoip.Business.Services
{
    public class DriverCommunicationService : IDriverCommunicationService
    {
        private UdpClient _udpClient;
        private WaveInEvent _waveIn;
        private IPEndPoint _endPoint;

        private WaveOutEvent waveOut;
        private UdpClient udpListeningClient;
        private CancellationTokenSource cancellationTokenSource;

        public void StopSpeakingWithPassenger(string ip)
        {
            if (_waveIn != null)
            {
                _waveIn.StopRecording();
                _waveIn.Dispose();
                _waveIn = null;
                Console.WriteLine($"{ip} ile konuşma durduruldu.");
            }

            if (_udpClient != null)
            {
                _udpClient.Dispose();
                _udpClient = null;
                Console.WriteLine($"UDP bağlantısı kapatıldı.");
            }
        }

        public void StartSpeakingWithPassenger(string ip, int speakPort, int listenPort)
        {
            if (_udpClient == null)
            {
                string serverIP = ip;
                int serverPort = listenPort;

                _udpClient = new UdpClient();
                _endPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
                Console.WriteLine($"UDP sunucusu {serverPort} portunda konuşuyor...");
                // Mikrofon girişini başlat
                _waveIn = new WaveInEvent
                {
                    WaveFormat = new WaveFormat(8000, 16, 1) // 8 kHz, 16 bit, mono
                };

                // Mikrofon verisini okuduğumuzda bu veriyi UDP üzerinden sunucuya gönderiyoruz
                _waveIn.DataAvailable += (sender, e) =>
                {
                    _udpClient.Send(e.Buffer, e.BytesRecorded, _endPoint);
                    Console.WriteLine($"Veri gönderildi: {e.BytesRecorded} byte");
                };

                _waveIn.StartRecording();
                Console.Out.WriteLineAsync("UDP istemcisi başlatıldı. Mikrofon verisi gönderiliyor...");
            }
        }


        public void StartListeningWithPassenger(string ip, int speakPort, int listenPort)
        {
            string serverIP = ip;
            int port = speakPort;
            udpListeningClient = new UdpClient(port);
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), port);
            Console.WriteLine($"UDP sunucusu {port} portunda dinliyor...");

            waveOut = new WaveOutEvent();
            var bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
            waveOut.Init(bufferedWaveProvider);
            waveOut.Play();

            cancellationTokenSource = new CancellationTokenSource();

            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                if (udpListeningClient.Available > 0)
                {
                    byte[] receivedBytes = udpListeningClient.Receive(ref remoteEndPoint);
                    Console.WriteLine($"Veri alındı: {receivedBytes.Length} byte");
                    bufferedWaveProvider.AddSamples(receivedBytes, 0, receivedBytes.Length);
                }
                else
                {
                    Task.Delay(10).Wait(); // Küçük bir bekleme, CPU yükünü azaltmak için
                }
            }
        }

        public void StopListeningWithPassenger(string ip)
        {
            cancellationTokenSource?.Cancel();

            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
                Console.WriteLine($"{ip} ile dinleme durduruldu.");
            }

            if (udpListeningClient != null)
            {
                udpListeningClient.Close();
                udpListeningClient.Dispose();
                udpListeningClient = null;
                Console.WriteLine($"UDP dinleme bağlantısı kapatıldı.");
            }
        }


    }


}

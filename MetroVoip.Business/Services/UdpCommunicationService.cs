using MetroVoip.Business.Interfaces;
using MetroVoip.Common.DTOs;
using NAudio.Wave;
using System.Net;
using System.Net.Sockets;

namespace MetroVoip.Business.Services
{
    public class UdpCommunicationService : IUdpCommunicationService
    {
        private UdpClient _udpClient;
        private bool _isListening;
        public async Task StartListeningAsync(int listenPort)
        {
            _udpClient = new UdpClient(listenPort);
            _isListening = true;

            await Task.Run(async () =>
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
                Console.WriteLine($"Listening on port {listenPort}");

                // Ses verisini hoparlörde çalmak için gerekli yapı
                var waveOut = new WaveOutEvent();
                var bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(8000, 16, 1)); // 8kHz, 16bit, mono
                waveOut.Init(bufferedWaveProvider);
                waveOut.Play();

                while (_isListening)
                {
                    byte[] receivedBytes = _udpClient.Receive(ref remoteEndPoint);
                    Console.WriteLine($"Veri alındı: {receivedBytes.Length} byte");

                    bufferedWaveProvider.AddSamples(receivedBytes, 0, receivedBytes.Length);
                }
            });
        }
        public async Task StopListeningAsync(int carriageNumber)
        {
            _isListening = false;
            _udpClient.Close();
            await Task.CompletedTask;
        }
        public async Task SendDataAsync(UdpCommunicationDto dto)
        {
            using (var udpClient = new UdpClient())
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(dto.IpAddress), dto.Port);
                await udpClient.SendAsync(dto.Data, dto.Data.Length, endPoint);
                Console.WriteLine($"Veri gönderildi: {dto.Data.Length} byte");
            }
        }
        
    }
}

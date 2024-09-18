using MetroVoip.Business.Interfaces;
using NAudio.Wave;
using System.Net;
using System.Net.Sockets;

namespace MetroVoip.Business.Services
{
    public class CommunicationService : ICommunicationService
    {
        private UdpClient udpClient;
        private WaveInEvent waveIn;
        private WaveOutEvent waveOut;
        private BufferedWaveProvider bufferedWaveProvider;
        private IPEndPoint localEndPoint;
        private bool isListening;

        // Stop the communication
        public void StopSpeaking(string ip)
        {
            // Stop speaking
            if (waveIn != null)
            {
                waveIn.StopRecording();
                waveIn.Dispose();
                waveIn = null;
                Console.WriteLine($"{ip} ile konuşma durduruldu.");
            }

            // Stop listening
            if (isListening && udpClient != null)
            {
                udpClient.Close();
                udpClient.Dispose();
                udpClient = null;
                isListening = false;
                Console.WriteLine($"{ip} ile dinleme durduruldu.");
            }

            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
                bufferedWaveProvider = null;
            }
        }

        public void StartSpeaking(string ip, int rPort, int sPort)
        {
            if (udpClient == null)
            {
                string serverIP = ip;
                int serverPort = sPort;

                // Create UdpClient for both sending and receiving
                udpClient = new UdpClient(rPort);
                localEndPoint = new IPEndPoint(IPAddress.Any, rPort);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

                // Configure microphone input
                waveIn = new WaveInEvent
                {
                    WaveFormat = new WaveFormat(8000, 16, 1) // 8 kHz, 16 bit, mono
                };

                // Send microphone data via UDP
                waveIn.DataAvailable += (sender, e) =>
                {
                    udpClient.Send(e.Buffer, e.BytesRecorded, endPoint);
                    Console.WriteLine($"{ip} ile veri gönderildi: {e.BytesRecorded} byte");
                };

                // Start recording from microphone
                waveIn.StartRecording();

                // Start listening to incoming audio data
                isListening = true;
                Task.Run(() =>
                {
                    while (isListening)
                    {
                        try
                        {
                            byte[] receivedData = udpClient.Receive(ref localEndPoint);
                            PlayReceivedAudio(receivedData);
                            Console.WriteLine($"{ip} ile veri alındı: {receivedData.Length} byte");
                        }
                        catch (ObjectDisposedException)
                        {
                            break;
                        }
                    }
                });

                // Initialize WaveOutEvent and BufferedWaveProvider for playback
                waveOut = new WaveOutEvent();
                bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
                waveOut.Init(bufferedWaveProvider);
                waveOut.Play();
            }
        }

        private void PlayReceivedAudio(byte[] audioData)
        {
            // Play received audio data
            if (bufferedWaveProvider != null)
            {
                bufferedWaveProvider.AddSamples(audioData, 0, audioData.Length);
            }
        }
    }


}

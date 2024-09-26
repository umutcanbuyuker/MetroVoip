using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NAudio.Wave;
using System.Net;
using System.Net.Sockets;

namespace MetroVoip.ListenerConsole
{
    public class Program
    {
        private static HubConnection connection;

        private static WaveOutEvent waveOut;
        private static UdpClient udpListeningClient;
        private static CancellationTokenSource cancellationTokenSource;

        private static UdpClient _udpClient;
        private static WaveInEvent _waveIn;
        private static IPEndPoint _endPoint;
        static async Task Main(string[] args) // Main metodunu async olarak tanımlayın
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .Build();

            var config = host.Services.GetService(typeof(IConfiguration)) as IConfiguration;
            var user = config.GetSection("User").Get<User>();

            connection = new HubConnectionBuilder()
                .WithUrl(user.RequestIpAddress) // Hub URL'nizi buraya yazın
                .WithAutomaticReconnect()
                .Build();

            // Bağlantıyı başlat
            await StartConnectionAsync(user);

            // Kapandığında yeniden bağlanma olayını ayarlayın
            connection.Closed += async (error) =>
            {
                Console.WriteLine("Bağlantı kapandı. Yeniden bağlanıyor...");
                await StartConnectionAsync(user);
            };

            connection.On<ManagerInfo>("StartListening", (info) =>
            {
                Console.WriteLine("StartListening " +info.IpAddress);
                StartListeningWithDriver(info);
            });

            connection.On<bool>("StopListening", (info) =>
            {
                StopListeningWithDriver();
            });

            connection.On<ManagerInfo>("StartSpeaking", (info) =>
            {
                Console.WriteLine("StartSpeaking " + info.IpAddress);
                StartSpeakingWithDriver(info);
            });

            connection.On<bool>("StopSpeaking", (info) =>
            {
                StopSpeakingWithDriver();
            });
            Console.ReadKey();
        }

        static async Task StartConnectionAsync(User user)
        {
            while (true)
            {
                try
                {
                    await connection.StartAsync();
                    Console.WriteLine("Bağlantı başarılı.");
                    await connection.InvokeAsync("RegisterListener", user.IpAddress, user.SpeakPort, user.ListenPort);
                    break; // Bağlantı başarılı olursa döngüden çık
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Bağlantı hatası: {ex.Message}. Yeniden deniyor...");
                    await Task.Delay(5000); // 5 saniye beklemeden sonra yeniden dene
                }
            }
        }

        public static void StopSpeakingWithDriver()
        {
            if (_waveIn != null)
            {
                _waveIn.StopRecording();
                _waveIn.Dispose();
                _waveIn = null;
                Console.WriteLine($"konuşma durduruldu.");
            }

            if (_udpClient != null)
            {
                _udpClient.Dispose();
                _udpClient = null;
                Console.WriteLine($"UDP bağlantısı kapatıldı.");
            }
        }

        public static void StartSpeakingWithDriver(ManagerInfo info /*string ip, int rPort, int sPort*/)
        {
            if (_udpClient == null)
            {

                string serverIP = info.IpAddress;
                int serverPort = info.ListenPort;

                _udpClient = new UdpClient();
                _endPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

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


        public static void StartListeningWithDriver(ManagerInfo info/*string ip, int rPort, int sPort*/)
        {

            string serverIP = info.IpAddress;
            int listenPort = info.SpeakPort;

            udpListeningClient = new UdpClient(listenPort);
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), listenPort);
            Console.WriteLine($"UDP sunucusu {listenPort} portunda dinliyor...");

            waveOut = new WaveOutEvent();
            var bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(8000, 16, 1));
            waveOut.Init(bufferedWaveProvider);
            waveOut.Play();

            cancellationTokenSource = new CancellationTokenSource();

            Task.Run(async () =>
            {
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
                        await Task.Delay(10); // Küçük bir bekleme, CPU yükünü azaltmak için
                    }
                }
            });
        }

        public static void StopListeningWithDriver()
        {
            cancellationTokenSource?.Cancel();

            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
                Console.WriteLine($"dinleme durduruldu.");
            }

            if (udpListeningClient != null)
            {
                udpListeningClient.Close();
                udpListeningClient.Dispose();
                udpListeningClient = null;
                Console.WriteLine($"UDP dinleme bağlantısı kapatıldı.");
            }
        }

        public class User
        {
            public string RequestIpAddress { get; set; }
            public string IpAddress { get; set; }
            public int SpeakPort { get; set; }
            public int ListenPort { get; set; }
        }

        public class ManagerInfo
        {
            public string IpAddress { get; set; }
            public int SpeakPort { get; set; }
            public int ListenPort { get; set; }
        }

    }

}
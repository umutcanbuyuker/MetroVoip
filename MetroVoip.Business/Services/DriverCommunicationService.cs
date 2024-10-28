using MetroVoip.Business.Interfaces;
using NAudio.Wave;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MetroVoip.Business.Services
{
    public class DriverCommunicationService : IDriverCommunicationService
    {
        private UdpClient udpClient;
        private WaveInEvent waveIn;
        private const string ServerIP = "10.1.58.85";
        private const int ServerPort = 5060;
        private int toTag;
        private MemoryStream audioBuffer;
        // Cabin's IPs
        private static Dictionary<int, string> kabinIPs = new Dictionary<int, string>
        {
            { 1, "10.2.149.24" },
            { 2, "10.2.20.27" },
            { 3, "127.0.0.2" },
            { 4, "127.0.0.3" }
        };

        public async Task StartSpeakingWithPassenger()
        {
            //if (udpClient == null)
            //{
            //    //string serverIP = kabinIPs[kabinId];
            //    string serverIP = "10.1.58.85";
            //    int serverPort = 5060;

            //    udpClient = new UdpClient();
            //    IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

            //    waveIn = new WaveInEvent
            //    {
            //        WaveFormat = new WaveFormat(8000, 16, 1) // 8 kHz, 16 bit, mono
            //    };

            //    // Mikrofon verisini UDP üzerinden gönderme
            //    waveIn.DataAvailable += async (sender, e) =>
            //    {
            //        await udpClient.SendAsync(e.Buffer, e.BytesRecorded, endPoint);
            //        Console.WriteLine($"Kabin ile veri gönderildi: {e.BytesRecorded} byte");
            //    };

            //    waveIn.StartRecording();
            //}
        }

        // Stop the communication with passenger.
        public async Task StopSpeakingWithPassenger(int kabinId)
        {
            //if (waveIn != null)
            //{
            //    waveIn.StopRecording();
            //    waveIn.Dispose();
            //    waveIn = null;
            //    Console.WriteLine($"Kabin {kabinId} ile konuşma durduruldu.");
            //}

            //if (udpClient != null)
            //{
            //    udpClient.Dispose();
            //    udpClient = null;
            //    Console.WriteLine($"UDP bağlantısı kapatıldı.");
            //}

            //await Task.CompletedTask;
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

        public async Task StartSipCall()
        {
            using (UdpClient udpClient = new UdpClient())
            {

                string sipMessage = @"INVITE sip:23@10.1.58.85 SIP/2.0
Via: SIP/2.0/UDP 10.1.58.86:65119;rport;branch=z9hG4bKPj5d74dd9a7e87479bafbc23a2c547dd55
Max-Forwards: 70
From: <sip:admin@10.1.58.85>;tag=6930f97b2a5e4cd9b45c643312f1b696
To: <sip:23@10.1.58.85>
Contact: <sip:admin@10.1.58.86:65119;ob>
Call-ID: 00a57ac84c7f4c29aece388f5bee29b3
CSeq: 2843 INVITE
Allow: PRACK, INVITE, ACK, BYE, CANCEL, UPDATE, INFO, SUBSCRIBE, NOTIFY, REFER, MESSAGE, OPTIONS
Supported: replaces, 100rel, timer, norefersub
Session-Expires: 1800
Min-SE: 90
User-Agent: MicroSIP/3.21.5
Content-Type: application/sdp
Content-Length:   336

v=0
o=- 3939101195 3939101195 IN IP4 10.2.149.10
s=pjmedia
b=AS:84
t=0 0
a=X-nat:0
m=audio 4064 RTP/AVP 8 0 101
c=IN IP4 10.2.149.10
b=TIAS:64000
a=rtcp:4065 IN IP4 10.2.149.10
a=sendrecv
a=rtpmap:8 PCMA/8000
a=rtpmap:0 PCMU/8000
a=rtpmap:101 telephone-event/8000
a=fmtp:101 0-16
a=ssrc:1903311638 cname:62d42ead13086271
";

                byte[] messageBytes = Encoding.UTF8.GetBytes(sipMessage);

                // Connect the UDP client to the SIP server
                udpClient.Connect("10.1.58.85", 5060);

                // Send the SIP INVITE message
                await udpClient.SendAsync(messageBytes, messageBytes.Length);
                Console.WriteLine("SIP INVITE message sent.");

                // Continuously listen for the 200 OK response
                while (true)
                {
                    var receivedResult = await udpClient.ReceiveAsync();
                    string responseMessage = Encoding.UTF8.GetString(receivedResult.Buffer);

                    Console.WriteLine("Received SIP Response: " + responseMessage);

                    // Look for "200 OK" response in the message
                    if (responseMessage.Contains("SIP/2.0 200 OK"))
                    {

                        var match = System.Text.RegularExpressions.Regex.Match(responseMessage, @"To:.*?tag=(\d+)");
                        string toTag = match.Success ? match.Groups[1].Value : "";

                        string sipAckMessage = "ACK sip:23@10.1.58.85:5060 SIP/2.0\r\n" +
                     "Via: SIP/2.0/UDP 10.1.58.86:65119;branch=z9hG4bKPjf6c3ea54c9c54f0c806a151483cd4e90\r\n" +
                     "From: <sip:admin@10.1.58.85>;tag=6930f97b2a5e4cd9b45c643312f1b696\r\n" +
                     "To: <sip:23@10.1.58.85;tag="+toTag+">\r\n" +
                     "Call-ID: 00a57ac84c7f4c29aece388f5bee29b3\r\n" +
                     "CSeq: 2843 ACK\r\n" +
                     "Contact: <sip:admin@10.1.58.86:65119;ob>\r\n\r\n";

                        byte[] ackMessageBytes = Encoding.UTF8.GetBytes(sipAckMessage);

                        // Send ACK message to confirm the call
                        await udpClient.SendAsync(ackMessageBytes, ackMessageBytes.Length);
                        Console.WriteLine("SIP ACK message sent.");

                        await StartAudioTransmission();

                        break;
                    }
                }
            }
        }

        private async Task StartAudioTransmission()
        {
            InitializeAudioCapture();

            uint timestamp = 800; // Example timestamp; update as needed
            ushort sequenceNumber = 1; // Start with sequence number 1

            while (true) // Loop to continuously send RTP packets
            {
                // Replace this with your actual audio data retrieval logic
                //byte[] audioData = new byte[160];

                byte[] audioData = GetAudioData();
                // Send RTP packet
                if (audioData.Length > 0)
                {
                    // Send RTP packet
                    await SendRtpPacket(audioData, timestamp, sequenceNumber);

                    // Increment the timestamp and sequence number for the next packet
                    timestamp += (uint)(audioData.Length * 1000000 / 8000); // Adjust according to your sampling rate
                    sequenceNumber++;
                }

                // Add a delay as needed (e.g., based on your audio frame duration)
                await Task.Delay(20); // Adjust this delay to control the sending rate
            }
        }

        private void InitializeAudioCapture()
        {
            waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(8000, 16, 1) // 8000 Hz, 16-bit, mono
            };
            waveIn.DataAvailable += OnDataAvailable;
            audioBuffer = new MemoryStream();
            waveIn.StartRecording();
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            audioBuffer.Write(e.Buffer, 0, e.BytesRecorded);
        }

        private byte[] GetAudioData()
        {
            // Get the audio data from the buffer
            byte[] data = audioBuffer.ToArray();
            audioBuffer.SetLength(0); // Clear the buffer after retrieval
            return data;
        }

        private async Task SendRtpPacket(byte[] audioData, uint timestamp, ushort sequenceNumber)
        {
            using (UdpClient rtpUdpClient = new UdpClient())
            {
                rtpUdpClient.Connect("10.1.58.85", 16384);
                // Construct RTP header
                byte[] rtpHeader = new byte[12];

                rtpHeader[0] = 0x80; // Version 2, no padding, no extension, 0 CSRC
                rtpHeader[1] = 0x08; // Payload type (8 for PCMA)
                rtpHeader[2] = (byte)(sequenceNumber >> 8); // Sequence number (high byte)
                rtpHeader[3] = (byte)(sequenceNumber & 0xFF); // Sequence number (low byte)
                rtpHeader[4] = (byte)(timestamp >> 24); // Timestamp (high byte)
                rtpHeader[5] = (byte)((timestamp >> 16) & 0xFF); // Timestamp (mid byte)
                rtpHeader[6] = (byte)((timestamp >> 8) & 0xFF); // Timestamp (mid low byte)
                rtpHeader[7] = (byte)(timestamp & 0xFF); // Timestamp (low byte)
                rtpHeader[8] = 0; // SSRC (high byte)
                rtpHeader[9] = 0; // SSRC (mid high byte)
                rtpHeader[10] = 0; // SSRC (mid low byte)
                rtpHeader[11] = 0; // SSRC (low byte)

                // Combine RTP header and audio data
                byte[] rtpPacket = new byte[rtpHeader.Length + audioData.Length];
                Buffer.BlockCopy(rtpHeader, 0, rtpPacket, 0, rtpHeader.Length);
                Buffer.BlockCopy(audioData, 0, rtpPacket, rtpHeader.Length, audioData.Length);

                // Send RTP packet to the intercom device
                await rtpUdpClient.SendAsync(rtpPacket, rtpPacket.Length);
                Console.WriteLine("RTP packet sent.");
            }
        }
        
        public async void EndSipCall()
        {
            try
            {
                using (UdpClient udpClient = new UdpClient())
                {
                    // BYE mesajını oluştur
                    string byeMessage = "BYE sip:23@10.1.58.85 SIP/2.0\r\n" +
                       "Via: SIP/2.0/UDP 10.1.58.86:65119;branch=z9hG4bKPjf6c3ea54c9c54f0c806a151483cd4e90\r\n" +
                       "From: <sip:admin@10.1.58.85>;tag=6930f97b2a5e4cd9b45c643312f1b696\r\n" +
                       "To: <sip:23@10.1.58.85>;tag="+toTag+">\r\n" + // toTag değeri burada dinamik olarak eklenebilir
                       "Call-ID: 00a57ac84c7f4c29aece388f5bee29b3\r\n" +
                       "CSeq: 2844 BYE\r\n" +
                       "Contact: <sip:admin@10.1.58.86:65119;ob>\r\n" +
                       "Content-Length: 0\r\n\r\n";


                    byte[] byeBytes = Encoding.UTF8.GetBytes(byeMessage);

                    // BYE mesajını gönder
                    await udpClient.SendAsync(byeBytes, byeBytes.Length, "10.1.58.85", 5060);
                    Console.WriteLine("BYE mesajı gönderildi. Görüşme sonlandırıldı.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata oluştu: " + ex.Message);
            }
        }

    }
}

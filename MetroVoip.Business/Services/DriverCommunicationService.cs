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
        private UdpClient rtpUdpClient;
        private WaveInEvent waveIn;
        private ushort sequenceNumber;
        private uint timestamp;

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
                     "To: <sip:23@10.1.58.85;tag=" + toTag + ">\r\n" +
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

        public async Task StartAudioTransmission()
        {
            // UdpClient'ı burada oluşturun ve yapılandırın
            rtpUdpClient = new UdpClient();
            await Task.Run(() => rtpUdpClient.Connect("10.1.58.85", 16384)); // Hedef IP ve port

            waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(8000, 16, 1) // 8 kHz, 16 bit, mono
            };
            waveIn.DataAvailable += WaveIn_DataAvailable; // Ses verisi geldiğinde çağrılacak metod
            waveIn.StartRecording(); // Ses kaydını başlat

            Console.WriteLine("Audio transmission started. Press Enter to stop.");
            Console.ReadLine(); // Kullanıcıdan durdurma için girdi bekle

            waveIn.StopRecording(); // Ses kaydını durdur
            waveIn.Dispose();
            rtpUdpClient.Dispose();
            Console.WriteLine("Audio transmission completed.");
        }

        private async void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            // 16-bit PCM formatındaki ham ses verisini 8-bit G.711 PCMA'ya dönüştür
            byte[] g711Data = Convert16BitPcmToG711(e.Buffer);

            // Dönüştürülmüş veriyi RTP paketiyle gönder
            await SendRtpPacket(g711Data, timestamp, sequenceNumber);

            // Timestamp ve sequence number'ı güncelle
            timestamp += (uint)g711Data.Length; // Bu örnekte her byte başına 1 artış
            sequenceNumber++;
        }

        private byte[] Convert16BitPcmToG711(byte[] pcmData)
        {
            int length = pcmData.Length / 2;
            byte[] g711Data = new byte[length];
            for (int i = 0; i < length; i++)
            {
                short sample = (short)((pcmData[i * 2 + 1] << 8) | pcmData[i * 2]);
                g711Data[i] = LinearToALawSample(sample);
            }
            return g711Data;
        }

        private byte LinearToALawSample(short sample)
        {
            // G.711 PCMA dönüştürme işlemi burada yapılır
            int sign = (sample >> 8) & 0x80;
            if (sign != 0) sample = (short)-sample;
            if (sample > 32635) sample = 32635;

            int exponent = 7;
            for (int expMask = 0x4000; (sample & expMask) == 0 && exponent > 0; exponent--, expMask >>= 1) ;
            int mantissa = (sample >> ((exponent == 0) ? 4 : (exponent + 3))) & 0x0F;
            byte alawByte = (byte)(sign | (exponent << 4) | mantissa);
            return (byte)~alawByte;
        }

        private async Task SendRtpPacket(byte[] audioData, uint timestamp, ushort sequenceNumber)
        {
            // RTP header oluşturma
            byte[] rtpHeader = new byte[12];
            rtpHeader[0] = 0x80; // Version 2, no padding, no extension, 0 CSRC 
            rtpHeader[1] = 0x08; // Payload type (8 for PCMA)
            rtpHeader[1] |= 0x80; // Set Marker bit to 1
            rtpHeader[2] = (byte)(sequenceNumber >> 8); // Sequence number (high byte)
            rtpHeader[3] = (byte)(sequenceNumber & 0xFF); // Sequence number (low byte)
            rtpHeader[4] = (byte)(timestamp >> 24); // Timestamp (high byte)
            rtpHeader[5] = (byte)((timestamp >> 16) & 0xFF); // Timestamp (mid byte)
            rtpHeader[6] = (byte)((timestamp >> 8) & 0xFF); // Timestamp (mid low byte)
            rtpHeader[7] = (byte)(timestamp & 0xFF); // Timestamp (low byte)

            uint ssrc = 0x3f2d5269; // SSRC değeri
            rtpHeader[8] = (byte)(ssrc >> 24); // SSRC (high byte)
            rtpHeader[9] = (byte)((ssrc >> 16) & 0xFF); // SSRC (mid high byte)
            rtpHeader[10] = (byte)((ssrc >> 8) & 0xFF); // SSRC (mid low byte)
            rtpHeader[11] = (byte)(ssrc & 0xFF); // SSRC (low byte)

            // RTP header ve ses verilerini birleştir
            byte[] rtpPacket = new byte[rtpHeader.Length + audioData.Length];
            Buffer.BlockCopy(rtpHeader, 0, rtpPacket, 0, rtpHeader.Length);
            Buffer.BlockCopy(audioData, 0, rtpPacket, rtpHeader.Length, audioData.Length);

            // RTP paketini gönder
            await rtpUdpClient.SendAsync(rtpPacket, rtpPacket.Length);

            // Hata ayıklama için gönderilen bilgileri yazdır
            Console.WriteLine($"RTP packet sent. Sequence Number: {sequenceNumber}, Timestamp: {timestamp}, SSRC: {ssrc:X}");
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
                       "To: <sip:23@10.1.58.85>;tag=" + toTag + ">\r\n" + // toTag değeri burada dinamik olarak eklenebilir
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

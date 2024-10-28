using MetroVoip.Business.Interfaces;
using NAudio.Wave;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;

namespace MetroVoip.Business.Services
{
    public class CommendIntercomService : ICommendIntercomService
    {
        private UdpClient udpClient;
        private UdpClient rtpClient;
        private WaveInEvent waveIn;

        public async Task SendSipInviteAsync(string sipUri, string driverIpAddress, int driverSipPort)
        {
            udpClient = new UdpClient();

            string sipMessage = $@"INVITE sip:{sipUri} SIP/2.0
Via: SIP/2.0/UDP {driverIpAddress}:{driverSipPort};rport;branch=z9hG4bKPj5d74dd9a7e87479bafbc23a2c547dd55
Max-Forwards: 70
From: <sip:admin@{driverIpAddress}>;tag=6930f97b2a5e4cd9b45c643312f1b696
To: <sip:{sipUri}>
Contact: <sip:admin@{driverIpAddress}:{driverSipPort};ob>
Call-ID: 00a57ac84c7f4c29aece388f5bee29b3
CSeq: 2843 INVITE
Allow: PRACK, INVITE, ACK, BYE, CANCEL, UPDATE, INFO, SUBSCRIBE, NOTIFY, REFER, MESSAGE, OPTIONS
Supported: replaces, 100rel, timer, norefersub
Session-Expires: 1800
Min-SE: 90
User-Agent: CustomSIP/1.0
Content-Type: application/sdp
Content-Length: 336

v=0
o=- 3938751481 3938751481 IN IP4 {driverIpAddress}
s=CustomMedia
b=AS:84
t=0 0
a=X-nat:0
m=audio 4016 RTP/AVP 8 0 101
c=IN IP4 {driverIpAddress}
b=TIAS:64000
a=rtcp:4017 IN IP4 {driverIpAddress}
a=sendrecv
a=rtpmap:8 PCMA/8000
a=rtpmap:0 PCMU/8000
a=rtpmap:101 telephone-event/8000
a=fmtp:101 0-16
a=ssrc:353727221 cname:13fc47934af81b77
";

            byte[] messageBytes = Encoding.UTF8.GetBytes(sipMessage);

            // UDP client connects to SIP server
            udpClient.Connect(driverIpAddress, 5060); // Intercom SIP server's IP and port

            // Send the SIP INVITE message
            await udpClient.SendAsync(messageBytes, messageBytes.Length);

            Console.WriteLine("SIP INVITE sent.");
        }

        public async Task StartRtpAudioStreamAsync(string remoteIpAddress, int remoteRtpPort)
        {
            rtpClient = new UdpClient();

            int localRtpPort = 4016; // Your local RTP port
            rtpClient.Client.Bind(new IPEndPoint(IPAddress.Any, localRtpPort));

            // Connect to remote RTP endpoint (Intercom's RTP port)
            rtpClient.Connect(remoteIpAddress, remoteRtpPort);

            // Start capturing audio from microphone
            waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(8000, 16, 1) // G.711 format
            };

            waveIn.DataAvailable += async (sender, args) =>
            {
                byte[] audioBuffer = args.Buffer;

                // RTP packet creation
                byte[] rtpPacket = CreateRtpPacket(audioBuffer);

                // Send RTP packet
                await rtpClient.SendAsync(rtpPacket, rtpPacket.Length);
            };

            waveIn.StartRecording();
        }

        public async Task StartRtpReceiverAsync(int localRtpPort)
        {
            UdpClient rtpReceiver = new UdpClient(localRtpPort);
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                var result = await rtpReceiver.ReceiveAsync();
                byte[] rtpPacket = result.Buffer;

                // Process RTP packet and extract the audio
                byte[] audioPayload = ExtractAudioPayloadFromRtpPacket(rtpPacket);

                // Play the audio (NAudio or any other)
                PlayAudio(audioPayload);
            }
        }

        public Task StopCallAsync()
        {
            waveIn?.StopRecording();
            waveIn?.Dispose();
            udpClient?.Close();
            rtpClient?.Close();
            Console.WriteLine("Call stopped.");
            return Task.CompletedTask;
        }

        private byte[] CreateRtpPacket(byte[] audioData)
        {
            // RTP header and audio data concatenation logic here
            // Example placeholder:
            byte[] rtpHeader = new byte[12]; // RTP header size
            byte[] rtpPacket = new byte[audioData.Length + rtpHeader.Length];
            Buffer.BlockCopy(rtpHeader, 0, rtpPacket, 0, rtpHeader.Length);
            Buffer.BlockCopy(audioData, 0, rtpPacket, rtpHeader.Length, audioData.Length);
            return rtpPacket;
        }

        private byte[] ExtractAudioPayloadFromRtpPacket(byte[] rtpPacket)
        {
            // Extract RTP header and get the payload
            // Example placeholder:
            int headerSize = 12; // Typically 12 bytes for RTP header
            int payloadSize = rtpPacket.Length - headerSize;
            byte[] audioPayload = new byte[payloadSize];
            Buffer.BlockCopy(rtpPacket, headerSize, audioPayload, 0, payloadSize);
            return audioPayload;
        }

        private void PlayAudio(byte[] audioData)
        {
            // Play received audio logic (using NAudio or any other library)
            Console.WriteLine("Playing audio...");
        }
    }
}

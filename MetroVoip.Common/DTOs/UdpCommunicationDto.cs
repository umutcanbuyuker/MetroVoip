namespace MetroVoip.Common.DTOs
{
    public class UdpCommunicationDto
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public byte[] Data { get; set; }
    }
}

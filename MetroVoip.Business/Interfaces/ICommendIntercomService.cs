namespace MetroVoip.Business.Interfaces
{
    public interface ICommendIntercomService
    {
        Task SendSipInviteAsync(string sipUri, string driverIpAddress, int driverSipPort);
        Task StartRtpAudioStreamAsync(string remoteIpAddress, int remoteRtpPort);
        Task StartRtpReceiverAsync(int localRtpPort);
        Task StopCallAsync(); // Optional to stop/terminate call
    }
}

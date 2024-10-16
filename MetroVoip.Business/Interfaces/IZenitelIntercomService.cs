namespace MetroVoip.Business.Interfaces
{
    public interface IZenitelIntercomService
    {
        Task AcceptCallAsync(int stationId);
        Task CancelCallAsync(int stationId);
        Task DeclineCallAsync(int stationId);
        Task SwitchRelayOutputAsync(int stationId, int relayId, bool activate);
        Task SimulateKeyPressAsync(int stationId, int keyCode);
        Task EstablishCallAsync(int sourceStationId, int targetStationId);
        Task TriggerSequenceAsync(int stationId, int sequenceId);
        Task ShowVideoStreamAsync(int stationId);
        Task GetStationStateAsync(int stationId);
    }
}

using MetroVoip.Business.Interfaces;
using System.Net.Http;

namespace MetroVoip.Business.Services
{
    public class ZenitInterkomService : IZenitelIntercomService
    {
        private readonly HttpClient _httpClient;

        public ZenitInterkomService(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }

        public async Task AcceptCallAsync(int stationId)
        {
            var url = $"https://{GetIntercomUrl(stationId)}/api/call/accept";
            await _httpClient.GetAsync(url);
        }

        public async Task CancelCallAsync(int stationId)
        {
            var url = $"https://{GetIntercomUrl(stationId)}/api/call/cancel";
            await _httpClient.GetAsync(url);
        }

        public async Task DeclineCallAsync(int stationId)
        {
            var url = $"https://{GetIntercomUrl(stationId)}/api/call/decline";
            await _httpClient.GetAsync(url);
        }

        public async Task SwitchRelayOutputAsync(int stationId, int relayId, bool activate)
        {
            var url = $"https://{GetIntercomUrl(stationId)}/api/relay/{relayId}/{(activate ? "on" : "off")}";
            await _httpClient.GetAsync(url);
        }

        public async Task SimulateKeyPressAsync(int stationId, int keyCode)
        {
            var url = $"https://{GetIntercomUrl(stationId)}/api/keypress/{keyCode}";
            await _httpClient.GetAsync(url);
        }

        public async Task EstablishCallAsync(int sourceStationId, int targetStationId)
        {
            var url = $"https://{GetIntercomUrl(sourceStationId)}/api/call/establish/{targetStationId}";
            await _httpClient.GetAsync(url);
        }

        public async Task TriggerSequenceAsync(int stationId, int sequenceId)
        {
            var url = $"https://{GetIntercomUrl(stationId)}/api/sequence/{sequenceId}";
            await _httpClient.GetAsync(url);
        }

        public async Task ShowVideoStreamAsync(int stationId)
        {
            var url = $"https://{GetIntercomUrl(stationId)}/api/video/stream";
            await _httpClient.GetAsync(url);
        }

        public async Task GetStationStateAsync(int stationId)
        {
            var url = $"https://{GetIntercomUrl(stationId)}/api/station/state";
            await _httpClient.GetAsync(url);
        }

        private string GetIntercomUrl(int stationId)
        {
            // Burada istasyon URL'sini oluşturma mantığı yer alabilir
            return $"intercom-device-ip-{stationId}"; // Örnek URL formatı
        }
    }
}

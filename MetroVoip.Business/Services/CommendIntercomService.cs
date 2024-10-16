using MetroVoip.Business.Interfaces;
using System.Net.Http;
using System.Text;

namespace MetroVoip.Business.Services
{
    public class CommendIntercomService : ICommendIntercomService
    {
        private readonly HttpClient _httpClient;
        public CommendIntercomService(HttpClient  httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task AcceptIncomingCall(string ipAddress, string username, string password)
        {
            try
            {
                // Accept call URL structure
                string url = $"http://{username}:{password}@{ipAddress}/cgi-bin/remotecontrol/hook.cgi?hook=answer";

                // Send HTTP GET request to accept the call
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Arama kabul edildi.");
                }
                else
                {
                    Console.WriteLine($"Arama kabul edilemedi. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata oluştu: {ex.Message}");
            }
        }
        public async Task CancelCall(string ipAddress, string username, string password)
        {
            var url = $"http://{username}:{password}@{ipAddress}/cgi-bin/remotecontrol/hook.cgi?hook=cancel";
            using var httpClient = new HttpClient();
            await httpClient.GetAsync(url);
        }

        public async Task DeclineCall(string ipAddress, string username, string password)
        {
            var url = $"http://{username}:{password}@{ipAddress}/cgi-bin/remotecontrol/hook.cgi?hook=decline";
            using var httpClient = new HttpClient();
            await httpClient.GetAsync(url);
        }
        public async Task<bool> AcceptIncomingCallAsync(string ipAddress, string username, string password)
        {
            var requestUrl = $"http://{ipAddress}/cgi-bin/remotecontrol/hook.cgi?hook=answer";

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

            try
            {
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine(ex.Message);
                return false;
            }
        }

    }
}

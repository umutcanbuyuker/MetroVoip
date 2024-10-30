using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SipTest
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Calling...");

            string username = "admin";  // Kullanıcı adı
            string password = "commend"; // Şifre
            string destination = "user@domain"; // Çağrı yapılacak kullanıcı bilgisi (örn: "user@domain", "user@domain:port")

            using (HttpClient client = new HttpClient())
            {
                // Sunucuya bağlantı ayarları
                string url = "http://10.1.58.85/cgi-bin/phonebook.cgi"; // Formun gönderileceği URL
                var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                // Başlıkları ayarlama
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36");

                // Çağrı için gerekli form verilerini oluşturma
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("destination", destination), // Çağrı yapılacak kullanıcı bilgisi
                    new KeyValuePair<string, string>("action", "webCall"), // Çağrı eylemi
                    new KeyValuePair<string, string>("CallOrHangup", "Call") // Çağrıyı başlat
                });

                // POST isteği gönderme
                HttpResponseMessage response = await client.PostAsync(url, formData);

                // İsteğin sonucunu kontrol etme
                if (response.IsSuccessStatusCode)
                {
                    // Başarılı yanıtı okuma
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response Body: " + responseBody);
                }
                else
                {
                    Console.WriteLine("Error: " + response.StatusCode);
                    // Hata mesajını kontrol etme
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Error Message: " + errorMessage);
                }
            }
        }
    }
}
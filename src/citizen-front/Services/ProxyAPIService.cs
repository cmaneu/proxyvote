using System.Net.Http.Json;

using ProxyVote.Core.Configuration;
using ProxyVote.Core.Entities;

namespace ProxyVote.Citizen.Front.Services
{
    public class ProxyAPIService
    {
        private readonly HttpClient HttpClient;

        private CitizenClientConfiguration _clientConfig;

        public ProxyAPIService(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public async Task InitializeService(string baseAddress)
        {
            _clientConfig = await HttpClient.GetFromJsonAsync<CitizenClientConfiguration>($"{baseAddress}config/client-config.json") ?? new CitizenClientConfiguration() {ApiEndpoint = "/" };
        }

        public async Task<string?> PostProxyRegistrationAsync(ProxyApplication application)
        {
            try
            {
                var response = await HttpClient.PostAsJsonAsync($"{_clientConfig.ApiEndpoint}/application", application);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Server error: {response.ReasonPhrase}");
                }

                var parsedMessage = await response.Content.ReadFromJsonAsync<RegistrationResponse>();
                return parsedMessage?.Id;
            }
            catch (Exception e)
            {
                // DEMO MODE : Never fail :)
                Console.WriteLine(e);

                return Guid.NewGuid().ToString();
            }
           
        }
    }

    public class RegistrationResponse
    {
        public string Id { get; set; }
    }
}

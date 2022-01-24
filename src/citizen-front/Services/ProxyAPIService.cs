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

        public async Task InitializeService()
        {
            _clientConfig = await HttpClient.GetFromJsonAsync<CitizenClientConfiguration>("config/client-config.json") ?? new CitizenClientConfiguration() {ApiEndpoint = "/" };
        }

        public async Task<string?> PostProxyRegistrationAsync(ProxyRegistration registration)
        {
            var response = await HttpClient.PostAsJsonAsync($"{_clientConfig.ApiEndpoint}/registration", registration);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Server error: {response.ReasonPhrase}");
            }

            var parsedMessage = await response.Content.ReadFromJsonAsync<RegistrationResponse>();
            return parsedMessage?.Id;
        }
    }

    public class RegistrationResponse
    {
        public string Id { get; set; }
    }
}

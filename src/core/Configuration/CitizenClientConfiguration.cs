using System.Text.Json.Serialization;

namespace ProxyVote.Core.Configuration
{
    public class CitizenClientConfiguration
    {
        [JsonPropertyName("api.endpoint")]
        public string ApiEndpoint { get; set; }
    }
}

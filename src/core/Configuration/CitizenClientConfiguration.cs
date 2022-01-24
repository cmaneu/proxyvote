using System.Text.Json.Serialization;

namespace ProxyVote.Core.Configuration
{
    public class CitizenClientConfiguration
    {
        [JsonPropertyName("ApiEndpoint")]
        public string ApiEndpoint { get; set; }
    }
}

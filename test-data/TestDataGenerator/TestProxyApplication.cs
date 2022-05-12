using ProxyVote.Core.Entities;

namespace TestDataGenerator;

public class TestProxyApplication
{
    public string id => $"ProxyApplication|{RegistrationId}";
    public string PartitionKey => $"2022-{Applicant.PostalCode[..2]}";
    public string? RegistrationId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Applicant Applicant { get; set; } = new Applicant();
    public ProxyVoter ProxyVoter { get; set; } = new ProxyVoter();
    public DateTime? ValidUntil { get; set; }
}
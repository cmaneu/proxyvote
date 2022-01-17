namespace ProxyVote.Core.Entities;

public class ProxyRegistration
{
    public string? RegistrationId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Applicant Applicant { get; set; } = new Applicant();
    public ProxyVoter ProxyVoter { get; set; } = new ProxyVoter();
    public DateTime? ValidUntil { get; set; }
}

namespace ProxyVote.Core.Entities;

public class ProxyVoteRegistration
{
    public string? RegistrationId { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public Applicant Applicant { get; set; } = new Applicant();
    public ProxyVoter ProxyVoter { get; set; } = new ProxyVoter();
    public DateTime? ValidUntil { get; set; }
    public ApplicationValidation Validation { get; set; }
}
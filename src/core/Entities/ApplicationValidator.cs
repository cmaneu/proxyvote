namespace ProxyVote.Core.Entities;

public class ApplicationValidation
{
    public string Type { get; set; }
    public string ValidationData { get; set; }
    public string ValidatorId { get; set; }
    public DateTime ValidationTime { get; set; }
}

namespace ProxyVote.Core.Entities;

public class Applicant
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public DateTime BirthDate { get; set; }
    public string StreetAddress { get; set; }
    public string State { get; set; }
    public string CityName { get; set; }
    public string PostalCode { get; set; }
}

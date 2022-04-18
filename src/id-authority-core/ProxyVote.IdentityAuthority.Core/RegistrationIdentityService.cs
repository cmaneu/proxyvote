using Microsoft.Azure.Cosmos;

using ProxyVote.Core.Entities;

namespace ProxyVote.IdentityAuthority.Core;

public class RegistrationIdentityService
{
    private static readonly string databaseId = "proxyvote";
    private static readonly string applicationsContainerId = "Applications";
    private static readonly string registrationsContainerId = "Registrations";

    private CosmosClient _cosmosClient;

    public RegistrationIdentityService(string endpoint, string accessKey)
    {
        _cosmosClient = new CosmosClient(endpoint, accessKey);
    }

    public async Task<ProxyApplication> GetRegistrationById(string department, string registrationId)
    {
        var db = _cosmosClient.GetDatabase(databaseId);
        var container = db.GetContainer(applicationsContainerId);

        // Note that Reads require a partition key to be specified.
        ItemResponse<ProxyApplication> response = await container.ReadItemAsync<ProxyApplication>(
            partitionKey: new PartitionKey(department),
            id: $"{nameof(ProxyApplication)}|{registrationId}");

        return response.Resource;
    }


    public async Task ValidateRegistration(string department, string registrationId, ApplicationValidation validation)
    {
        var application = await GetRegistrationById(department, registrationId);

        validation.ValidationTime = DateTime.Now;
        validation.Type = "Human|PoliceStation";

        ProxyVoteRegistration registration = new ProxyVoteRegistration()
        {
            Applicant = application.Applicant,
            ProxyVoter = application.ProxyVoter,
            RegistrationId = registrationId,
            ReceivedAt = application.CreatedAt,
            Validation = validation,
            ValidUntil = application.ValidUntil
        };

        var db = _cosmosClient.GetDatabase(databaseId);
        var container = db.GetContainer(registrationsContainerId);

        await container.CreateItemAsync(new ProxyVoteItem(registration), new PartitionKey(department));
    }
}


internal class ProxyVoteItem
{
    public string id { get; set; }
    public string PartitionKey { get; set; }
    public ProxyVoteRegistration Registration { get; set; } 
    public string Status { get; set; }

    public ProxyVoteItem()
    {
        
    }

    public ProxyVoteItem(ProxyVoteRegistration registration)
    {
        Registration = registration;
        id = registration.RegistrationId;
        PartitionKey = $"{registration.ReceivedAt.Value.Year}-{registration.Applicant.PostalCode[..2]}";
        Status = "Received";
    }
}

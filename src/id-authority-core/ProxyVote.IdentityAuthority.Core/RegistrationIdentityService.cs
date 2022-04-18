using Microsoft.Azure.Cosmos;

using ProxyVote.Core.Entities;

namespace ProxyVote.IdentityAuthority.Core;

public class RegistrationIdentityService
{
    private static readonly string databaseId = "proxyvote";
    private static readonly string containerId = "Applications";

    private CosmosClient _cosmosClient;

    public RegistrationIdentityService(string endpoint, string accessKey)
    {
        _cosmosClient = new CosmosClient(endpoint, accessKey);
    }

    public async Task<ProxyApplication> GetRegistrationById(string department, string registrationId)
    {
        var db = _cosmosClient.GetDatabase(databaseId);
        var container = db.GetContainer(containerId);

        // Note that Reads require a partition key to be specified.
        ItemResponse<ProxyApplication> response = await container.ReadItemAsync<ProxyApplication>(
            partitionKey: new PartitionKey(department),
            id: $"{nameof(ProxyApplication)}|{registrationId}");

        return response.Resource;
    }



}

using Microsoft.EntityFrameworkCore;

using ProxyVote.Core.Entities;
using ProxyVote.Core.Services;

namespace ProxyVote.Core.Adapters;

public class ProxyCosmosDbProvider : IProxyDbProvider
{
    private readonly IDbContextFactory<ProxyDBContext> _factory;

    public ProxyCosmosDbProvider(IDbContextFactory<ProxyDBContext> factory)
    {
        _factory = factory;
    }

    public async Task InsertProxyRegistrationAsync(ProxyRegistration registration)
    {
        await using var context = await _factory.CreateDbContextAsync();

        context.SetPartitionKey(registration, ComputePartitionKey(registration));
        context.Add(registration);

        await context.SaveChangesAsync();
    }
    
    public string ComputePartitionKey(ProxyRegistration registration) => $"{registration.CreatedAt.Value.Year}-{registration.Applicant.PostalCode.Substring(0,2)}";
}


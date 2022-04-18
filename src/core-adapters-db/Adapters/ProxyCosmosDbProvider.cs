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

    public async Task InsertProxyRegistrationAsync(ProxyApplication application)
    {
        await using var context = await _factory.CreateDbContextAsync();

        context.SetPartitionKey(application, ComputePartitionKey(application));
        context.Add(application);

        await context.SaveChangesAsync();
    }

    public string ComputePartitionKey(ProxyApplication application) => $"{application.CreatedAt.Value.Year}-{application.Applicant.PostalCode.Substring(0,2)}";
}


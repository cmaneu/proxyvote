using ProxyVote.Core.Entities;

namespace ProxyVote.Core.Services;

public class ProxyRegistrationService
{

    private readonly IProxyDbProvider _proxyDbProvider;

    public ProxyRegistrationService(IProxyDbProvider proxyDbProvider)
    {
        _proxyDbProvider = proxyDbProvider;
    }

    public async Task<string> CreateRegistrationAsync(ProxyApplication application)
    {
        application.RegistrationId = Guid.NewGuid().ToString("N");
        application.CreatedAt = DateTime.UtcNow;
        
        // TODO: Validate entity

        await _proxyDbProvider.InsertProxyRegistrationAsync(application);

        return application.RegistrationId;
    }
}

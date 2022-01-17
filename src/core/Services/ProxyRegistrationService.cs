using ProxyVote.Core.Entities;

namespace ProxyVote.Core.Services;

public class ProxyRegistrationService
{

    private readonly IProxyDbProvider _proxyDbProvider;

    public ProxyRegistrationService(IProxyDbProvider proxyDbProvider)
    {
        _proxyDbProvider = proxyDbProvider;
    }

    public async Task<string> CreateRegistrationAsync(ProxyRegistration registration)
    {
        registration.RegistrationId = Guid.NewGuid().ToString("N");
        registration.CreatedAt = DateTime.UtcNow;
        
        // TODO: Validate entity

        await _proxyDbProvider.InsertProxyRegistrationAsync(registration);

        return registration.RegistrationId;
    }
}

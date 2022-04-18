using ProxyVote.Core.Entities;

namespace ProxyVote.Core.Services;

public interface IProxyDbProvider
{
    Task InsertProxyRegistrationAsync(ProxyApplication application);
}

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy
{
    public interface IDesktopClientProxyFactory : IClientProxyFactory
    {
        IClientProxy<TChannel> GetDuplexClientProxy<TChannel>(object callbackImplementation);
    }
}
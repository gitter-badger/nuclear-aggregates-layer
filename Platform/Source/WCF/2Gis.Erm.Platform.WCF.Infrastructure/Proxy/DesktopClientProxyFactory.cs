using System.ServiceModel;

using DoubleGis.Erm.Platform.WCF.Infrastructure.Config;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy
{
    public class DesktopClientProxyFactory : ClientProxyFactory, IDesktopClientProxyFactory
    {
        public DesktopClientProxyFactory(IServiceClientSettingsProvider serviceClientSettingsProvider) 
            : base(serviceClientSettingsProvider)
        {
        }

        public IClientProxy<TChannel> GetDuplexClientProxy<TChannel>(object callbackImplementation)
        {
            var factory = new DuplexChannelFactory<TChannel>(new InstanceContext(callbackImplementation),
                                                             ServiceClientSettingsProvider.GetEndpoint(typeof(TChannel), typeof(WSDualHttpBinding)));

            return new ClientProxy<TChannel>(factory.CreateChannel());
        }
    }
}
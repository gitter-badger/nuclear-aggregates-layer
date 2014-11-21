using System.Configuration;
using System.ServiceModel.Channels;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy
{
    public interface IClientProxyFactory
    {
        IClientProxy<TChannel> GetClientProxy<TChannel, TBinding>()
            where TBinding : Binding;

        IClientProxy<TChannel> GetClientProxy<TChannel>(string configurationEndpointName);

        IClientProxy<TChannel> GetClientProxy<TChannel>(string endpointConfigurationName, Configuration configuration);
    }
}
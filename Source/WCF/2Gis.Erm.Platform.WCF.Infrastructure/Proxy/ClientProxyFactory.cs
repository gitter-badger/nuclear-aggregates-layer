using System.ServiceModel;
using System.ServiceModel.Channels;

using DoubleGis.Erm.Platform.WCF.Infrastructure.Config;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy
{
    public class ClientProxyFactory : IClientProxyFactory
    {
        private readonly IServiceClientSettingsProvider _serviceClientSettingsProvider;

        public ClientProxyFactory(IServiceClientSettingsProvider serviceClientSettingsProvider)
        {
           _serviceClientSettingsProvider = serviceClientSettingsProvider;
        }

        protected IServiceClientSettingsProvider ServiceClientSettingsProvider
        {
            get { return _serviceClientSettingsProvider; }
        }

        public IClientProxy<TChannel> GetClientProxy<TChannel, TBinding>() where TBinding : Binding
        {
            var factory = new ChannelFactory<TChannel>(ServiceClientSettingsProvider.GetEndpoint(typeof(TChannel), typeof(TBinding)));
            return new ClientProxy<TChannel>(factory.CreateChannel());
        }

        public IClientProxy<TChannel> GetClientProxy<TChannel>(string endpointConfigurationName)
        {
            var factory = new ChannelFactory<TChannel>(endpointConfigurationName);
            var channel = factory.CreateChannel();
            
            return new ClientProxy<TChannel>(channel);
        }
    }
}
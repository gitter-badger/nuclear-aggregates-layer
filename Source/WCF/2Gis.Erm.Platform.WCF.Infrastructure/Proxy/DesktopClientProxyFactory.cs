using System.ServiceModel;
using System.ServiceModel.Channels;

using DoubleGis.Erm.Platform.WCF.Infrastructure.Config;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.SharedTypes;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy
{
    public class DesktopClientProxyFactory : IDesktopClientProxyFactory
    {
        private readonly ISharedTypesBehaviorFactory _sharedTypesBehaviorFactory;
        private readonly IServiceClientSettingsProvider _serviceClientSettingsProvider;

        public DesktopClientProxyFactory(ISharedTypesBehaviorFactory sharedTypesBehaviorFactory, IServiceClientSettingsProvider serviceClientSettingsProvider)
        {
            _sharedTypesBehaviorFactory = sharedTypesBehaviorFactory;
            _serviceClientSettingsProvider = serviceClientSettingsProvider;
        }

        public IClientProxy<TChannel> GetClientProxy<TChannel, TBinding>()
            where TBinding : Binding
        {
            var factory = new ChannelFactory<TChannel>(_serviceClientSettingsProvider.GetEndpoint(typeof(TChannel), typeof(TBinding)));
            var channel = ConfigureFactoryAndCreateChannel(factory);

            return new ClientProxy<TChannel>(channel);
        }

        public IClientProxy<TChannel> GetClientProxy<TChannel>(string configurationEndpointName)
        {
            var factory = new ChannelFactory<TChannel>(configurationEndpointName);
            var channel = ConfigureFactoryAndCreateChannel(factory);

            return new ClientProxy<TChannel>(channel);
        }

        public IClientProxy<TChannel> GetDuplexClientProxy<TChannel>(object callbackImplementation)
        {
            var factory = new DuplexChannelFactory<TChannel>(new InstanceContext(callbackImplementation),
                                                             _serviceClientSettingsProvider.GetEndpoint(typeof(TChannel), typeof(WSDualHttpBinding)));
            var channel = ConfigureFactoryAndCreateChannel(factory);

            return new ClientProxy<TChannel>(channel);
        }

        private TChannel ConfigureFactoryAndCreateChannel<TChannel>(ChannelFactory<TChannel> channelFactory)
        {
            channelFactory.Endpoint.EndpointBehaviors.Add(_sharedTypesBehaviorFactory.Create());
            return channelFactory.CreateChannel();
        }
    }
}
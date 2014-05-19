using System;
using System.Configuration;
using System.ServiceModel.Channels;

using DoubleGis.Erm.Platform.API.ServiceBusBroker;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure.Fakes.Import.BrokerApiReceiver
{
    internal class FakeBrokerApiReceiverClientProxyFactory : IClientProxyFactory
    {
        public IClientProxy<TChannel> GetClientProxy<TChannel, TBinding>() where TBinding : Binding
        {
            throw new NotImplementedException();
        }

        public IClientProxy<TChannel> GetClientProxy<TChannel>(string configurationEndpointName)
        {
            if (typeof(TChannel) == typeof(IBrokerApiReceiver))
            {
                return (IClientProxy<TChannel>)new FakeBrokerApiReceiverProxy();
            }

            throw new NotImplementedException();
        }

        public IClientProxy<TChannel> GetClientProxy<TChannel>(string endpointConfigurationName, Configuration configuration)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.ServiceModel;

using DoubleGis.Erm.Platform.API.ServiceBusBroker;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure.Fakes.Import.BrokerApiReceiver
{
    internal class FakeBrokerApiReceiverProxy : IClientProxy<IBrokerApiReceiver>
    {
        private readonly FakeBrokerApiReciever _brokerApiReciever = new FakeBrokerApiReciever();

        public void Execute(Action<IBrokerApiReceiver> action)
        {
            action(_brokerApiReciever);
        }

        public TResult Execute<TResult>(Func<IBrokerApiReceiver, TResult> func)
        {
            return func(_brokerApiReciever);
        }

        public bool TryExecuteWithFaultContract<TResult>(Func<IBrokerApiReceiver, TResult> func, out TResult result, out object faultContract)
        {
            faultContract = null;
            try
            {
                result = func(_brokerApiReciever);
                return true;
            }
            catch (FaultException e)
            {
                result = default(TResult);
                return false;
            }
        }
    }
}
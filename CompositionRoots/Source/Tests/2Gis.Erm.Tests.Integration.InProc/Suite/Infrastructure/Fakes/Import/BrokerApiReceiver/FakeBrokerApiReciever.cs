using System;
using System.IO;
using System.Linq;

using DoubleGis.Erm.Platform.API.ServiceBusBroker;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure.Fakes.Import.BrokerApiReceiver
{
    internal class FakeBrokerApiReciever : IBrokerApiReceiver
    {
        private bool _acknowledgeCalled;
        private bool _endReceivingCalled;
        private string _messageType;

        public void BeginReceiving(string appCode, string messageType)
        {
            _acknowledgeCalled = false;
            _endReceivingCalled = false;
            _messageType = messageType;
        }

        public string[] ReceivePackage()
        {
            if (_endReceivingCalled)
            {
                throw new InvalidOperationException();
            }

            if (_acknowledgeCalled)
            {
                return null;
            }

            var directory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Suite\Infrastructure\Fakes\Import\Messages"));

            return directory.EnumerateFiles(string.Format(@"{0}.*.xml", _messageType))
                            .Select(fi => File.ReadAllText(fi.FullName))
                            .ToArray();
        }

        public void Acknowledge()
        {
            _acknowledgeCalled = true;
        }

        public void EndReceiving()
        {
            _endReceivingCalled = true;
        }
    }
}
namespace DoubleGis.Erm.Platform.Core.Messaging.Transports.ServiceBusForWindowsServer
{
    public sealed partial class ServiceBusMessageReceiver<TMessageFlow>
    {
        private readonly object _disposeSync = new object();
        private bool _isDisposed;
        
        private bool IsDisposed
        {
            get
            {
                lock (_disposeSync)
                {
                    return _isDisposed;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            lock (_disposeSync)
            {
                if (_isDisposed)
                {
                    return;
                }

                if (disposing)
                {
                    _serviceBusConnectionPool.Dispose();
                }

                // Free your own state (unmanaged objects).
                _isDisposed = true;
            }
        }
    }
}
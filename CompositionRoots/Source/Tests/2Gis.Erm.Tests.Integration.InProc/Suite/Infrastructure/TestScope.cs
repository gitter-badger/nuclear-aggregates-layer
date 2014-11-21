using System;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure
{
    public sealed class TestScope : IDisposable
    {
        private readonly IIntegrationTest _test;
        private readonly IUnityContainer _testContainer;

        public TestScope(IIntegrationTest test, IUnityContainer testContainer)
        {
            _test = test;
            _testContainer = testContainer;
        }

        public IIntegrationTest Test
        {
            get { return _test; }
        }

        #region Поддержка IDisposable

        private readonly object _disposeSync = new object();

        /// <summary>
        /// Флаг того что instance disposed
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// Флаг того что instance disposed - потокобезопасный
        /// </summary>
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

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Внутренний dispose класса
        /// </summary>
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
                    _testContainer.Dispose();
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.
                // TODO

                _isDisposed = true;
            }
        }

        #endregion
    }
}
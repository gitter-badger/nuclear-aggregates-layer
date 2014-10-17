namespace DoubleGis.Erm.BLCore.OrderValidation.Performance.Sessions
{
    public sealed partial class PerformanceCounterOrderValidationDiagnosticStorage
    {
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
                    // Free other state (managed objects).
                    foreach (var rulesCounterSetInstance in _rulesCounterSetInstances.Values)
                    {
                        rulesCounterSetInstance.Dispose();
                    }

                    foreach (var ruleGroupsCounterSetInstance in _ruleGroupsCounterSetInstances.Values)
                    {
                        ruleGroupsCounterSetInstance.Dispose();
                    }

                    _sessionsCounterSetInstance.Dispose();

                    _rulesCounterSet.Dispose();
                    _ruleGroupsCounterSet.Dispose();
                    _sessionsCounterSet.Dispose();
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.
                // TODO

                _isDisposed = true;
            }
        }
    }
}
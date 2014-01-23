using System;
using System.Threading;
using System.Threading.Tasks;

using DoubleGis.Erm.Qds.Etl.Flow;

namespace DoubleGis.Erm.Qds.IndexService
{
    public class BatchIndexingProcess : IIndexingProcess
    {
        private readonly IEtlFlow _etlFlow;
        private readonly BatchIndexingSettings _settings;
        private bool _executing;
        private Task _currentTask;
        private readonly object _sync = new object();
        private readonly int _sleep;

        public BatchIndexingProcess(IEtlFlow etlFlow, BatchIndexingSettings settings)
        {
            if (etlFlow == null)
            {
                throw new ArgumentNullException("etlFlow");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            _etlFlow = etlFlow;
            _settings = settings;
        }

        public void Start()
        {
            _etlFlow.Init();

            lock (_sync)
            {
                if (_executing)
                    return;

                _executing = true;
                _currentTask = Task.Run(() => ExecuteEtlFlowContinuously());
            }
        }

        public void Stop()
        {
            lock (_sync)
            {
                if (!_executing)
                    return;

                _executing = false;
                _currentTask.Wait(_settings.StopTimeout);

            }
        }

        void ExecuteEtlFlowContinuously()
        {
            while (_executing)
            {
                _etlFlow.Execute();
                Thread.Sleep(_settings.SleepTime);
            }
        }
    }
}
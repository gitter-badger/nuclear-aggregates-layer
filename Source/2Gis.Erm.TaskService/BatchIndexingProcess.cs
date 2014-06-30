using System;
using System.Threading;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Etl.Flow;
using DoubleGis.Erm.TaskService.Settings;

namespace DoubleGis.Erm.TaskService
{
    public sealed class BatchIndexingProcess : IIndexingProcess
    {
        private readonly IEtlFlow _etlFlow;
        private readonly IBatchIndexingSettings _settings;
        readonly ICommonLog _logger;
        private bool _executing;
        private Task _currentTask;
        private readonly object _sync = new object();

        public BatchIndexingProcess(IEtlFlow etlFlow, IBatchIndexingSettings settings, ICommonLog logger)
        {
            if (etlFlow == null)
            {
                throw new ArgumentNullException("etlFlow");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _etlFlow = etlFlow;
            _settings = settings;
            _logger = logger;
        }

        public void Start()
        {
            try
            {
                _etlFlow.Init();
            }
            catch (Exception ex)
            {
                _logger.ErrorEx(ex, "Cannot initialize ETL flow.");
            }

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
                try
                {
                    _etlFlow.Execute();
                }
                catch (TrackerStateException ex)
                {
                    _logger.WarnFormatEx("Cannot get last replicated PBO id. Maybe not held data replication after migration. Exception: {0}", ex);
                }
                catch (Exception ex)
                {
                    _logger.ErrorEx(ex, "Unknown error during replication of changes.");
                }

                Thread.Sleep(_settings.SleepTime);
            }
        }
    }
}
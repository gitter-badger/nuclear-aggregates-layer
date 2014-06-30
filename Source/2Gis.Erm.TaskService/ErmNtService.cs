using System;
using System.ServiceProcess;

using DoubleGis.Erm.Platform.TaskService.Schedulers;

namespace DoubleGis.Erm.TaskService
{
    internal sealed partial class ErmNtService : ServiceBase
    {
        private readonly ISchedulerManager _schedulerManager;
        readonly IIndexingProcess _indexingProcess;

        public ErmNtService(ISchedulerManager schedulerManager, IIndexingProcess indexingProcess)
        {
            if (schedulerManager == null)
            {
                throw new ArgumentNullException("schedulerManager");
            }

            _schedulerManager = schedulerManager;
            _indexingProcess = indexingProcess;

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            _schedulerManager.Start();

            if (_indexingProcess != null)
                _indexingProcess.Start();
        }

        protected override void OnStop()
        {
            base.OnStop();
            _schedulerManager.Stop();

            if (_indexingProcess != null)
                _indexingProcess.Stop();
        }
    }
}
using System.ServiceProcess;

using NuClear.Jobs.Schedulers;

namespace DoubleGis.Erm.TaskService
{
    internal sealed partial class ErmNtService : ServiceBase
    {
        private readonly ISchedulerManager _schedulerManager;

        public ErmNtService(ISchedulerManager schedulerManager)
        {
            _schedulerManager = schedulerManager;

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            _schedulerManager.Start();
        }

        protected override void OnStop()
        {
            base.OnStop();
            _schedulerManager.Stop();
        }
    }
}
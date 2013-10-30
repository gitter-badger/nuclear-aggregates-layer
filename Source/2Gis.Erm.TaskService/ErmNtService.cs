using System.ServiceProcess;

using DoubleGis.Erm.Platform.TaskService.Schedulers;

namespace DoubleGis.Erm.TaskService
{
    internal sealed partial class ErmNtService : ServiceBase
    {
        private readonly SchedulerManager _schedulerManager;

        public ErmNtService(SchedulerManager schedulerManager)
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
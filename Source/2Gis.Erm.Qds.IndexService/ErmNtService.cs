using System;
using System.ServiceProcess;

namespace DoubleGis.Erm.Qds.IndexService
{
    // TODO Точка входа сервиса
    internal sealed partial class ErmNtService : ServiceBase
    {
        private readonly IIndexingProcess _indexingProcess;

        public ErmNtService(IIndexingProcess indexingProcess)
        {
            if (indexingProcess == null)
            {
                throw new ArgumentNullException("indexingProcess");
            }

            _indexingProcess = indexingProcess;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            _indexingProcess.Start();
        }

        protected override void OnStop()
        {
            base.OnStop();
            _indexingProcess.Stop();
        }
    }
}
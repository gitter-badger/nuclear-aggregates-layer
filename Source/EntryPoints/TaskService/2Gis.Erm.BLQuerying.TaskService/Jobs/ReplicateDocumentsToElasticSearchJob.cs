using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.TaskService.Jobs;
using DoubleGis.Erm.Qds.API.Operations;

using Quartz;

namespace DoubleGis.Erm.BLQuerying.TaskService.Jobs
{
    [DisallowConcurrentExecution]
    public sealed class ReplicateDocumentsToElasticSearchJob : TaskServiceJobBase
    {
        private readonly IDefferedDocumentUpdater _defferedDocumentUpdater;

        public ReplicateDocumentsToElasticSearchJob(ISignInService signInService, IUserImpersonationService userImpersonationService, ICommonLog logger, IDefferedDocumentUpdater defferedDocumentUpdater)
            : base(signInService, userImpersonationService, logger)
        {
            _defferedDocumentUpdater = defferedDocumentUpdater;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _defferedDocumentUpdater.IndexAllDocuments();
        }
    }
}
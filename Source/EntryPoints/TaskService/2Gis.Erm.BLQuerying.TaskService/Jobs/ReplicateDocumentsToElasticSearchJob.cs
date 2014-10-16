﻿using System;
using System.Threading;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.TaskService.Jobs;
using DoubleGis.Erm.Qds.API.Operations.Indexing;

using Quartz;

namespace DoubleGis.Erm.BLQuerying.TaskService.Jobs
{
    [DisallowConcurrentExecution]
    public sealed class ReplicateDocumentsToElasticSearchJob : TaskServiceJobBase, IInterruptableJob
    {
        private readonly IDefferedDocumentUpdater _defferedDocumentUpdater;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public ReplicateDocumentsToElasticSearchJob(ISignInService signInService, IUserImpersonationService userImpersonationService, ICommonLog logger, IDefferedDocumentUpdater defferedDocumentUpdater)
            : base(signInService, userImpersonationService, logger)
        {
            _defferedDocumentUpdater = defferedDocumentUpdater;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            try
            {
                _defferedDocumentUpdater.IndexAllDocuments(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException) { }
        }

        public void Interrupt()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}
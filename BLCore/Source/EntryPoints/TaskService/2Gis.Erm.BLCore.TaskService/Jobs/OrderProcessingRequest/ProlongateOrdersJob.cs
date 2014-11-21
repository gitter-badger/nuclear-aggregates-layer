﻿using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.OrderProcessingRequest
{
    [DisallowConcurrentExecution]
    public sealed class ProlongateOrdersJob : TaskServiceJobBase
    {
        private readonly IProcessOrderProlongationRequestMassOperation _orderPrologationOperation;

        public ProlongateOrdersJob(ISignInService signInService,
                                   IUserImpersonationService userImpersonationService,
                                   ICommonLog logger,
                                   IProcessOrderProlongationRequestMassOperation orderPrologationOperation)
            : base(signInService, userImpersonationService, logger)
        {
            _orderPrologationOperation = orderPrologationOperation;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _orderPrologationOperation.ProcessAll();
        }
    }
}

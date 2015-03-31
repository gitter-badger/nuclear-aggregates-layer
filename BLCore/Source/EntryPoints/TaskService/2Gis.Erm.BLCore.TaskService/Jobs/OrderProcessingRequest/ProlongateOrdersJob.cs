using DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests;

using NuClear.Jobs;
using NuClear.Security.API;
using NuClear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.OrderProcessingRequest
{
    [DisallowConcurrentExecution]
    public sealed class ProlongateOrdersJob : TaskServiceJobBase
    {
        private readonly IProcessOrderProlongationRequestMassOperation _orderPrologationOperation;

        public ProlongateOrdersJob(ISignInService signInService,
                                   IUserImpersonationService userImpersonationService,
                                   ITracer tracer,
                                   IProcessOrderProlongationRequestMassOperation orderPrologationOperation)
            : base(signInService, userImpersonationService, tracer)
        {
            _orderPrologationOperation = orderPrologationOperation;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _orderPrologationOperation.ProcessAll();
        }
    }
}

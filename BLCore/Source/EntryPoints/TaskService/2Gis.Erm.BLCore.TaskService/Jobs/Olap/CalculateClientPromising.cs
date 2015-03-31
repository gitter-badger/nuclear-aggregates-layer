using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Olap;

using NuClear.Jobs;
using NuClear.Security.API;
using NuClear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.Olap
{
    public sealed class CalculateClientPromising : TaskServiceJobBase
    {
        private readonly ICalculateClientPromisingOperationService _calculateClientPromisingOperationService;

        public CalculateClientPromising(ISignInService signInService,
                                        IUserImpersonationService userImpersonationService,
                                        ITracer tracer,
                                        ICalculateClientPromisingOperationService calculateClientPromisingOperationService) : base(signInService, userImpersonationService, tracer)
        {
            _calculateClientPromisingOperationService = calculateClientPromisingOperationService;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _calculateClientPromisingOperationService.CalculateClientPromising();
        }
    }
}
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Olap;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Nuclear.Tracing.API;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.Olap
{
    public sealed class CalculateClientPromising : TaskServiceJobBase
    {
        private readonly ICalculateClientPromisingOperationService _calculateClientPromisingOperationService;

        public CalculateClientPromising(ISignInService signInService,
                                        IUserImpersonationService userImpersonationService,
                                        ICommonLog logger,
                                        ICalculateClientPromisingOperationService calculateClientPromisingOperationService) : base(signInService, userImpersonationService, logger)
        {
            _calculateClientPromisingOperationService = calculateClientPromisingOperationService;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _calculateClientPromisingOperationService.CalculateClientPromising();
        }
    }
}
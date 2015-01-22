using System.Collections.Generic;
using System.Threading;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using NuClear.Model.Common.Operations.Identity.Generic;

using Quartz;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs
{
    [DisallowConcurrentExecution]
    public sealed class PushFakeOperationsJob : TaskServiceJobBase, IInterruptableJob
    {
        private readonly IOperationScopeFactory _scopeFactory;

        private bool _isStopped;

        public PushFakeOperationsJob(
            IOperationScopeFactory scopeFactory,
            ISignInService signInService,
            IUserImpersonationService userImpersonationService,
            ICommonLog logger)
            : base(signInService, userImpersonationService, logger)
        {
            _scopeFactory = scopeFactory;
        }

        public void Interrupt()
        {
            LogInfo("Stopping ... ");
            _isStopped = true;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            while (!_isStopped)
            {
                LogInfo("Processing ... ");

                Process();

                Thread.Sleep(10);
            }

            LogInfo("Stopped ... ");
        }

        private void Process()
        {
            using (var scope = _scopeFactory.CreateNonCoupled<ImportFirmIdentity>())
            {
                using (var nestedScope = _scopeFactory.CreateSpecificFor<UpdateIdentity, FirmAddress>())
                {
                    nestedScope.Updated<FirmAddress>(UpdatedFirmAddresses())
                               .Complete();
                }

                scope.Updated<Firm>(UpdatedFirms())
                     .Complete();
            }
        }

        private IEnumerable<long> UpdatedFirms()
        {
            return new[]
                {
                    985699289399298,
                    985699289399353,
                    985699289399367,
                    985699289399371,
                    985699289399375,
                    985699289399377,
                    985699289399483,
                    985699289399516,
                    985699289399595,
                    985699289399600
                };
        }

        private IEnumerable<long> UpdatedFirmAddresses()
        {
            return new[]
                {
                    985690699464764,
                    985690699464778,
                    985690699464782,
                    985690699464786,
                    985690699464788,
                    985690699464899,
                    985690699464935,
                    985690699465018,
                    985690699465056,
                    985690699465073
                };
        }
    }
}
using DoubleGis.Erm.BL.API.Operations.Concrete.Limits;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Limits
{
    public class RecalculateLimitOperationServiceTest : UseModelEntityTestBase<Limit>
    {
        private readonly IRecalculateLimitOperationService _recalculateLimitOperationService;

        public RecalculateLimitOperationServiceTest(IAppropriateEntityProvider<Limit> appropriateEntityProvider, IRecalculateLimitOperationService recalculateLimitOperationService)
            : base(appropriateEntityProvider)
        {
            _recalculateLimitOperationService = recalculateLimitOperationService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(Limit modelEntity)
        {
            _recalculateLimitOperationService.Recalculate(modelEntity.Id);

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}
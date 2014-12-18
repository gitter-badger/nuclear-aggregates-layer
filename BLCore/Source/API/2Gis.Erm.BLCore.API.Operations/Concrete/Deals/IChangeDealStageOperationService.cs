using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Deal;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals
{
    public interface IChangeDealStageOperationService : IOperation<ChangeDealStageIdentity>
    {
        void Change(long dealId, DealStage dealStage);
    }
}

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Limit;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Limits
{
    // TODO {y.baranihin, 31.10.2014}: перенести в BL
    public interface IRecalculateLimitOperationService : IOperation<RecalculateLimitIdentity>
    {
        void Recalculate(long limitId);
    }
}
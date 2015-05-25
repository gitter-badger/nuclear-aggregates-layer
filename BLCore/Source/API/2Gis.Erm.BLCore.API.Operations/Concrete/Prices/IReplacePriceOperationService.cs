using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices
{
    public interface IReplacePriceOperationService : IOperation<ReplacePriceIdentity>
    {
        void Replace(long sourcePriceId, long targetPriceId);
    }
}
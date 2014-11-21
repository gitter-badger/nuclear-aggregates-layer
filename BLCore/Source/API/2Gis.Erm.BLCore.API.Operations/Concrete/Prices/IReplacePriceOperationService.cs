using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices
{
    public interface IReplacePriceOperationService : IOperation<ReplacePriceIdentity>
    {
        int Replace(long sourcePriceId, long targetPriceId);
    }
}
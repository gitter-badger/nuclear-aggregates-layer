using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices
{
    public interface ICopyPricePositionOperationService : IOperation<CopyPricePositionIdentity>
    {
        void Copy(long priceId, long sourcePricePositionId, long positionId);
    }
}
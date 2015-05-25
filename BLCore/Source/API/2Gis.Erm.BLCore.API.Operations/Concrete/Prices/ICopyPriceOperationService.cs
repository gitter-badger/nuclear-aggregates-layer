using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices
{
    public interface ICopyPriceOperationService : IOperation<CopyPriceIdentity>
    {
        void Copy(long sourcePriceId, long organizationUnitId, DateTime publishDate, DateTime beginDate);
        void Copy(long sourcePriceId, long organizationUnitId, DateTime createDate, DateTime publishDate, DateTime beginDate);
    }
}
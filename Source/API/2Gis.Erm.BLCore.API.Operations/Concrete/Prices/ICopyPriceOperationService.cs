﻿using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices
{
    public interface ICopyPriceOperationService : IOperation<CopyPriceIdentity>
    {
        int Copy(long sourcePriceId, long organizationUnitId, DateTime publishDate, DateTime beginDate);
        int Copy(long sourcePriceId, long organizationUnitId, DateTime createDate, DateTime publishDate, DateTime beginDate);
    }
}
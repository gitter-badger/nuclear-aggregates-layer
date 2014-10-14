﻿using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations
{
    public interface IDeleteLegalPersonDealService : IAggregateSpecificOperation<Deal, DeleteIdentity>
    {
        void Delete(LegalPersonDeal link, bool isLinkLastOne);
    }
}

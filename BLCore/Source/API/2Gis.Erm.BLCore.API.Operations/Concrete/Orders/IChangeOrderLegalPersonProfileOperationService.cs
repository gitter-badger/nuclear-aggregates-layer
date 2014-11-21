﻿using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    public interface IChangeOrderLegalPersonProfileOperationService : IOperation<ChangeOrderLegalPersonProfileIdentity>
    {
        void ChangeLegalPersonProfile(long orderId, long profileId);
    }
}

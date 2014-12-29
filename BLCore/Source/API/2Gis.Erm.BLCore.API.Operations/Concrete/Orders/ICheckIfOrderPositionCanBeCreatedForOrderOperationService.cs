﻿using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    public interface ICheckIfOrderPositionCanBeCreatedForOrderOperationService : IOperation<CheckIfOrderPositionCanBeCreatedForOrderIdentity>
    {
        bool Check(long orderId, OrderType orderType, out string report);
        bool Check(long orderId, long orderPositionId, long pricePositionId, IEnumerable<AdvertisementDescriptor> orderPositionAdvertisements, out string report);
    }
}
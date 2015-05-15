﻿using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Bills
{
    public interface IBulkDeleteBillAggregateService : IAggregateSpecificOperation<Order, BulkDeleteIdentity>
    {
        void DeleteBills(Order order, IEnumerable<Bill> bills);
    }
}

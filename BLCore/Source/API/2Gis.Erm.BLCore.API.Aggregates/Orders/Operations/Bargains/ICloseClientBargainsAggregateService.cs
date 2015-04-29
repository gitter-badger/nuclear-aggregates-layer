using System;
using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Bargain;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Bargains
{
    public interface ICloseClientBargainsAggregateService : IAggregateSpecificOperation<Order, BulkCloseClientBargainsIdentity>
    {
        void CloseBargains(IEnumerable<Bargain> bargains, DateTime closeDate);
    }
}
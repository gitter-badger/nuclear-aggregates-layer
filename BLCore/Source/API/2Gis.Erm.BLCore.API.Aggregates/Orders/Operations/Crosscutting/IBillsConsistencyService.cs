using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting
{
    public interface IBillsConsistencyService : IInvariantSafeCrosscuttingService
    {
        void Validate(IEnumerable<Bill> bills, Order order);
    }
}
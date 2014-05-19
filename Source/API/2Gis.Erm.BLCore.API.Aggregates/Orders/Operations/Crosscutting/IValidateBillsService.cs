using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting
{
    public interface IValidateBillsService : IInvariantSafeCrosscuttingService
    {
        bool PreValidate(IEnumerable<Bill> bills, out string report);
        bool Validate(IEnumerable<Bill> bills, out string report);
    }
}
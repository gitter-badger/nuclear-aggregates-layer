using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Crosscutting
{
    public interface IBillInvariant
    {
        bool Validate(IEnumerable<Bill> bills, Order order, out string report);
    }
}
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Multiculture.Crosscutting
{
    public class NullValidateBillsService : IValidateBillsService, IRussiaAdapted, ICyprusAdapted, ICzechAdapted
    {
        public bool PreValidate(IEnumerable<Bill> bills, out string report)
        {
            report = null;
            return true;
        }

        public bool Validate(IEnumerable<Bill> bills, out string report)
        {
            report = null;
            return true;
        }
    }
}
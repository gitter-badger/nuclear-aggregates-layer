using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

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
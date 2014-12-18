using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using DoubleGis.Erm.BLFlex.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Crosscutting
{
    public sealed class ChileBillNumberFormatConsistencyRule : IBillConsistencyRule
    {
        private static readonly Regex ChileBillNumberPattern = new Regex(@"^\d{7}$", RegexOptions.Compiled);

        public bool Validate(IEnumerable<Bill> bills, Order order, out string report)
        {
            var wrongBillNumbers = bills.Select(x => x.BillNumber)
                                        .Where(x => !ChileBillNumberPattern.IsMatch(x))
                                        .ToArray();
            if (wrongBillNumbers.Any())
            {
                report = string.Format(BLResources.WrongBillNumbers, string.Join(", ", wrongBillNumbers));
                return false;
            }

            report = null;
            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using DoubleGis.Erm.BLFlex.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Crosscutting
{
    public sealed class ChileBillNumberFormatInvariant : IBillInvariant
    {
        private static readonly Regex ChileBillNumberPattern = new Regex(@"^\d{7}$", RegexOptions.Compiled);

        public bool Validate(IEnumerable<Bill> bills, Order order, out string report)
        {
            var billNumbers = bills.Select(x => x.BillNumber).ToArray();
            var billIds = bills.Where(x => !x.IsNew()).Select(x => x.Id).ToArray();

            var wrongBillNumbers = billNumbers.Where(x => ChileBillNumberPattern.IsMatch(x)).ToArray();
            if (wrongBillNumbers.Any())
            {
                report = String.Format(BLResources.WrongBillNumbers, String.Join(", ", wrongBillNumbers));
                return false;
            }

            report = null;
            return true;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Bills;
using DoubleGis.Erm.BLFlex.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Crosscutting
{
    public class ChileValidateBillsService : IValidateBillsService, IChileAdapted
    {
        private static readonly Regex ChileBillNumberPattern = new Regex(@"^\d{7}$", RegexOptions.Compiled);
        private readonly IFinder _finder;

        public ChileValidateBillsService(IFinder finder)
        {
            _finder = finder;
        }

        public bool PreValidate(IEnumerable<Bill> bills, out string report)
        {
            report = null;
            var billNumbers = bills.Select(x => x.BillNumber).ToArray();

            var duplicateBillNumbers = billNumbers.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToArray();
            if (duplicateBillNumbers.Any())
            {
                report = string.Format(BLResources.DublicateBillNumbers, string.Join(", ", duplicateBillNumbers));
                return false;
            }

            var wrongBillNumbers = billNumbers.Where(x => ChileBillNumberPattern.IsMatch(x)).ToArray();
            if (wrongBillNumbers.Any())
            {
                report = string.Format(BLResources.WrongBillNumbers, string.Join(", ", wrongBillNumbers));
                return false;
            }

            return true;
        }

        public bool Validate(IEnumerable<Bill> bills, Order order, out string report)
        {
            report = null;
            var billNumbers = bills.Select(x => x.BillNumber).ToArray();
            var billIds = bills.Where(x => !x.IsNew()).Select(x => x.Id).ToArray();

            var existingNumbers = _finder.Find(Specs.Find.ActiveAndNotDeleted<Bill>() && BillSpecifications.Find.ByNumbers(billNumbers) && !Specs.Find.ByIds<Bill>(billIds))
                                         .Select(x => x.BillNumber)
                                         .ToArray();

            if (existingNumbers.Any())
            {
                report = string.Format(BLResources.ExistingBillNumbers, string.Join(", ", existingNumbers));
                return false;
            }

            foreach (var bill in bills)
            {
                var endOfPaymentDatePlan = bill.PaymentDatePlan.GetEndOfTheDay();
                var endOfCheckPeriod = bill.BeginDistributionDate.GetPrevMonthLastDate();
                if (order.SignupDate > bill.PaymentDatePlan && endOfPaymentDatePlan <= endOfCheckPeriod)
                {
                    report = BLCore.Resources.Server.Properties.BLResources.BillPaymentDatePlanMustBeInCorrectPeriod;
                    return false;
                }
            }

            return true;
        }
    }
}
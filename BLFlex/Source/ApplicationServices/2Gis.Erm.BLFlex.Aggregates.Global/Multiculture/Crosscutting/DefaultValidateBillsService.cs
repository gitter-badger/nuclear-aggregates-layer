using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Multiculture.Crosscutting
{
    public class DefaultValidateBillsService : IValidateBillsService, IRussiaAdapted, ICyprusAdapted, ICzechAdapted
    {
        private readonly IFinder _finder;

        public DefaultValidateBillsService(IFinder finder)
        {
            _finder = finder;
        }

        public bool PreValidate(IEnumerable<Bill> bills, out string report)
        {
            report = null;
            return true;
        }

        public bool Validate(IEnumerable<Bill> bills, out string report)
        {
            foreach (var bill in bills)
            {
                var orderInfo = _finder.Find(Specs.Find.ById<Order>(bill.OrderId))
                                       .Select(x => new
                                       {
                                           IsOrderActive = x.WorkflowStepId == OrderState.OnRegistration,
                                           SignupDate = x.SignupDate,
                                       })
                                       .Single();

                if (!orderInfo.IsOrderActive)
                {
                    report = BLCore.Resources.Server.Properties.BLResources.CantEditBillsWhenOrderIsNotOnRegistration;
                    return false;
                }

                var endOfPaymentDatePlan = new DateTime(bill.PaymentDatePlan.Year, bill.PaymentDatePlan.Month, bill.PaymentDatePlan.Day)
                                    .AddDays(1)
                                    .AddSeconds(-1);

                var endOfCheckPeriod = bill.BeginDistributionDate.AddMonths(-1).GetEndPeriodOfThisMonth();
                if (orderInfo.SignupDate > bill.PaymentDatePlan && endOfPaymentDatePlan <= endOfCheckPeriod)
                {
                    report = BLCore.Resources.Server.Properties.BLResources.BillPaymentDatePlanMustBeInCorrectPeriod;
                    return false;
                }
            }

            report = null;
            return true;
        }
    }
}
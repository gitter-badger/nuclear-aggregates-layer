using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Bills;
using DoubleGis.Erm.BLFlex.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Crosscutting
{
    public sealed class BillDublicateNumbersConsistencyRule : IBillConsistencyRule
    {
        private readonly IFinder _finder;

        public BillDublicateNumbersConsistencyRule(IFinder finder)
        {
            _finder = finder;
        }

        public bool Validate(IEnumerable<Bill> bills, Order order, out string report)
        {
            // дубликаты в пакете
            var billNumbers = bills.Select(x => x.Number).ToArray();
            var duplicateBillNumbers = billNumbers.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToArray();
            if (duplicateBillNumbers.Any())
            {
                report = String.Format(BLResources.DublicateBillNumbers, String.Join(", ", duplicateBillNumbers));
                return false;
            }

            // пересечения в базе с пакетом
            var billIds = bills.Where(x => !x.IsNew()).Select(x => x.Id).ToArray();
            var existingNumbers = _finder.Find(Specs.Find.ActiveAndNotDeleted<Bill>() && BillSpecifications.Find.ByNumbers(billNumbers) && !Specs.Find.ByIds<Bill>(billIds))
                                         .Select(x => x.Number)
                                         .ToArray();

            if (existingNumbers.Any())
            {
                report = String.Format(BLResources.ExistingBillNumbers, String.Join(", ", existingNumbers));
                return false;
            }

            report = null;
            return true;
        }
    }
}
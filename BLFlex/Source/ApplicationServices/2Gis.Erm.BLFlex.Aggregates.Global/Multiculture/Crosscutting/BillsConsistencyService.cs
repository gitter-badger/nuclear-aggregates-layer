using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Crosscutting
{
    public class BillsConsistencyService : IBillsConsistencyService
    {
        private readonly IEnumerable<IBillConsistencyRule> _rules;

        public BillsConsistencyService(IEnumerable<IBillConsistencyRule> rules)
        {
            _rules = rules;
        }

        public void Validate(IEnumerable<Bill> bills, Order order)
        {
            foreach (var rule in _rules)
            {
                string report;
                if (!rule.Validate(bills, order, out report))
                {
                    throw new BillsConsistencyException(report);
                }
            }
        }
    }
}
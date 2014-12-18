using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Crosscutting
{
    public class ValidateBillsService : IValidateBillsService
    {
        private readonly IEnumerable<IBillInvariant> _invariants;

        public ValidateBillsService(IEnumerable<IBillInvariant> invariants)
        {
            _invariants = invariants;
        }

        public void Validate(IEnumerable<Bill> bills, Order order)
        {
            string report;
            if (!Validate(bills, order, out report))
            {
                throw new NotificationException(report);
            }
        }

        public bool Validate(IEnumerable<Bill> bills, Order order, out string report)
        {
            report = null;
            foreach (var invariant in _invariants)
            {
                if (!invariant.Validate(bills, order, out report))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
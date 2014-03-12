using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Validation
{
    public sealed class ValidResultsContainer
    {
        private readonly IEnumerable<long> _orderIds;
        private readonly IDictionary<long, IEnumerable<long>> _ordersByFirm;
        private readonly IDictionary<long, long> _firmByOrder;
        private readonly List<ValidResult> _validResults;

        internal IEnumerable<ValidResult> ValidResults
        {
            get { return _validResults.AsReadOnly(); }
        }

        internal ValidResultsContainer(OrderToFirmMap[] orderToFirmMaps)
        {
            _validResults = new List<ValidResult>();

            _orderIds = orderToFirmMaps.Select(x => x.OrderId).ToArray();
            // ReSharper disable PossibleInvalidOperationException
            _ordersByFirm = orderToFirmMaps
                .Where(x => x.FirmId.HasValue)
                .GroupBy(x => x.FirmId.Value)
                .ToDictionary(x => x.Key, x => x.Select(y => y.OrderId));
            _firmByOrder = orderToFirmMaps.Where(x => x.FirmId.HasValue).ToDictionary(x => x.OrderId, x => x.FirmId.Value);
            // ReSharper restore PossibleInvalidOperationException
        }

        public void AppendValidResults(IEnumerable<long> invalidOrderIds, ValidationContext validationContext)
        {
            var extentedInvalidOrderIds = (from invalidOrderId in invalidOrderIds
                                           join firmByOrder in _firmByOrder on invalidOrderId equals firmByOrder.Key
                                           join ordersByFirmItem in _ordersByFirm on firmByOrder.Value equals ordersByFirmItem.Key
                                           from orderId in ordersByFirmItem.Value
                                           select orderId)
                .Distinct()
                .ToArray();

            var validOrderIds = _orderIds.Except(extentedInvalidOrderIds).ToArray();
            foreach (var validOrderId in validOrderIds)
            {
                _validResults.Add(new ValidResult {OrderId = validOrderId, ValidationContext = validationContext});
            }
        }
    }
}
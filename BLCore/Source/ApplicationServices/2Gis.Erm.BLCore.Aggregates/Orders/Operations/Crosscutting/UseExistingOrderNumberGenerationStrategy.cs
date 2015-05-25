using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting
{
    public sealed class UseExistingOrderNumberGenerationStrategy : IOrderNumberGenerationStrategy
    {
        public bool TryGenerateNumber(string currentOrderNumber, string orderNumberTemplate, long? reservedNumber, out string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(currentOrderNumber))
            {
                orderNumber = null;
                return false;
            }

            orderNumber = currentOrderNumber;
            return true;
        }
    }
}

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Crosscutting
{
    public sealed class UseReservedNumberOrderNumberGenerationStrategy : IOrderNumberGenerationStrategy
    {
        public bool TryGenerateNumber(string currentOrderNumber, string orderNumberTemplate, long? reservedNumber, out string orderNumber)
        {
            if (!reservedNumber.HasValue)
            {
                orderNumber = null;
                return false;
            }

            orderNumber = string.Format(orderNumberTemplate, reservedNumber.Value);
            return true;
        }
    }
}

using System.Text.RegularExpressions;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Crosscutting;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Orders
{
    public abstract class ReadFromFormatOrderNumberGenerationStrategy : IOrderNumberGenerationStrategy
    {
        public abstract bool TryGenerateNumber(string currentOrderNumber, string orderNumberTemplate, long? reservedNumber, out string orderNumber);
        
        protected bool ReadFromFormat(Regex formatRegex, string currentOrderNumber, string orderNumberTemplate, out string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(currentOrderNumber))
            {
                orderNumber = null;
                return false;
            }

            var match = formatRegex.Match(currentOrderNumber);
            var orderNumberMatchesFormat = match.Success && match.Groups.Count == 2;

            long orderNumberValue;
            if (!orderNumberMatchesFormat || !long.TryParse(match.Groups[1].Value, out orderNumberValue))
            {
                orderNumber = null;
                return false;
            }

            orderNumber = string.Format(orderNumberTemplate, orderNumberValue);
            return true;
        }
    }
}

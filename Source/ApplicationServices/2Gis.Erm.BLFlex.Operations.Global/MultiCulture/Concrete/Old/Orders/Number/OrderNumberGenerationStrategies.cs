using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.Number
{
    public delegate bool OrderNumberGenerationStrategy(string currentOrderNumber, string orderNumberTemplate, long? reservedNumber, out string orderNumber);

    public static class OrderNumberGenerationStrategies
    {
        public static readonly IEnumerable<OrderNumberGenerationStrategy> ForRussia = new OrderNumberGenerationStrategy[]
            {
                Russia.ReadFromNewFormat,
                Russia.ReadFromOldFormat,
                UseReservedNumber,
                UseExistingOrderNumber
            };

        public static readonly IEnumerable<OrderNumberGenerationStrategy> ForCountriesWithRomanAlphabet = new OrderNumberGenerationStrategy[]
            {
                MultiCulture.ReadFromCurrentOrderNumber,
                UseReservedNumber,
                UseExistingOrderNumber
            };
        
        public static bool UseReservedNumber(string currentOrderNumber, string orderNumberTemplate, long? reservedNumber, out string orderNumber)
        {
            if (!reservedNumber.HasValue)
            {
                orderNumber = null;
                return false;
            }

            orderNumber = string.Format(orderNumberTemplate, reservedNumber.Value);
            return true;
        }

        public static bool UseExistingOrderNumber(string currentOrderNumber, string orderNumberTemplate, long? reservedNumber, out string orderNumber)
        {
            if (string.IsNullOrWhiteSpace(currentOrderNumber))
            {
                orderNumber = null;
                return false;
            }

            orderNumber = currentOrderNumber;
            return true;
        }

        public static class Russia
        {
            private const string NewOrderNumberRegexTemplate = @"[à-ÿÀ-ß]+_\d+-\d+-(\d+)-?[a-zA-Z]*";
            private const string OldOrderNumberRegexTemplate = @"[à-ÿÀ-ß]+_\d+-\d+-(\d+)";

            public static bool ReadFromNewFormat(string currentOrderNumber, string orderNumberTemplate, long? reservedNumber, out string orderNumber)
            {
                var newOrderNumberRegex = new Regex(NewOrderNumberRegexTemplate, RegexOptions.Singleline | RegexOptions.Compiled);
                return ReadFromFormat(newOrderNumberRegex, currentOrderNumber, orderNumberTemplate, out orderNumber);
            }
        
            public static bool ReadFromOldFormat(string currentOrderNumber, string orderNumberTemplate, long? reservedNumber, out string orderNumber)
            {
                var oldOrderNumberRegex = new Regex(OldOrderNumberRegexTemplate, RegexOptions.Singleline | RegexOptions.Compiled);
                return ReadFromFormat(oldOrderNumberRegex, currentOrderNumber, orderNumberTemplate, out orderNumber);
            }
        }

        public static class MultiCulture
        {
            private const string OrderNumberRegexTemplate = @"[a-zA-Z]+_\d+-\d+-(\d+)";

            public static bool ReadFromCurrentOrderNumber(string currentOrderNumber, string orderNumberTemplate, long? reservedNumber, out string orderNumber)
            {
                var newOrderNumberRegex = new Regex(OrderNumberRegexTemplate, RegexOptions.Singleline | RegexOptions.Compiled);
                return ReadFromFormat(newOrderNumberRegex, currentOrderNumber, orderNumberTemplate, out orderNumber);
            }
        }

        private static bool ReadFromFormat(Regex formatRegex, string currentOrderNumber, string orderNumberTemplate, out string orderNumber)
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
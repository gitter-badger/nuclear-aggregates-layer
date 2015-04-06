using System.Text.RegularExpressions;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Orders
{
    public sealed class ReadFromCurrentOrderNumberGenerationStrategy : ReadFromFormatOrderNumberGenerationStrategy
    {
        private const string OrderNumberRegexTemplate = @"[a-zA-Z]+_\d+-\d+-(\d+)";

        public override bool TryGenerateNumber(string currentOrderNumber, string orderNumberTemplate, long? reservedNumber, out string orderNumber)
        {
            var newOrderNumberRegex = new Regex(OrderNumberRegexTemplate, RegexOptions.Singleline | RegexOptions.Compiled);
            return ReadFromFormat(newOrderNumberRegex, currentOrderNumber, orderNumberTemplate, out orderNumber);
        }
    }
}

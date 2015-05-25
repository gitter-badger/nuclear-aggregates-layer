using System.Text.RegularExpressions;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Orders
{
    public sealed class ReadFromNewFormatOrderNumberGenerationStrategy : ReadFromFormatOrderNumberGenerationStrategy
    {
        private const string NewOrderNumberRegexTemplate = @"[а-яА-Я]+_\d+-\d+-(\d+)-?[a-zA-Z]*";

        public override bool TryGenerateNumber(string currentOrderNumber, string orderNumberTemplate, long? reservedNumber, out string orderNumber)
        {
            var newOrderNumberRegex = new Regex(NewOrderNumberRegexTemplate, RegexOptions.Singleline | RegexOptions.Compiled);
            return ReadFromFormat(newOrderNumberRegex, currentOrderNumber, orderNumberTemplate, out orderNumber);
        }
    }
}

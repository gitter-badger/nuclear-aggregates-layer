using System.Text.RegularExpressions;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.MultiCulture.Orders
{
    public sealed class ReadFromOldFormatOrderNumberGenerationStrategy : ReadFromFormatOrderNumberGenerationStrategy
    {
        private const string OldOrderNumberRegexTemplate = @"[а-яА-Я]+_\d+-\d+-(\d+)";

        public override bool TryGenerateNumber(string currentOrderNumber, string orderNumberTemplate, long? reservedNumber, out string orderNumber)
        {
            var newOrderNumberRegex = new Regex(OldOrderNumberRegexTemplate, RegexOptions.Singleline | RegexOptions.Compiled);
            return ReadFromFormat(newOrderNumberRegex, currentOrderNumber, orderNumberTemplate, out orderNumber);
        }
    }
}

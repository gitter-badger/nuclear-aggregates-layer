using System.Linq;
using System.Text.RegularExpressions;

using DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.Platform.API.Core.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Crosscutting
{
    /// <summary>
    /// Проверяет корректность чилийского Rut,
    /// за основу взят алгоритм, описанный тут: http://www.vesic.org/english/blog/c-sharp/verifying-chilean-rut-code-tax-number/
    /// Сгенерировать валидные занчения можно тут: 
    /// </summary>
    public sealed class ChileRutService : ICheckInnService, IChileAdapted
    {
        private static readonly Regex RutFormat = new Regex(@"^(\d\d?)\.(\d\d\d)\.(\d\d\d)\-([0-9K])$", RegexOptions.Compiled);
        private static readonly int[] Multipliers = { 3, 2, 7, 6, 5, 4, 3, 2 };
        private static readonly char[] CheckDigitsForCheckSum = { '1', '2', '3', '4', '5', '6', '7', '8', '9', 'K', '0' };

        public bool TryGetErrorMessage(string rut, out string message)
        {
            var match = RutFormat.Match(rut);
            if (!match.Success)
            {
                // FIXME {all, 22.01.2014}: требуется локализация
                message = "Invalid RUT format. Expected something like 12.345.678-K";
                return true;
            }

            var digits = string.Concat(match.Groups[1], match.Groups[2], match.Groups[3]);
            if (digits.Length == 7)
            {
                digits = string.Concat("0", digits);
            }

            var sum = digits.Zip(Multipliers, (digit, multiplier) => int.Parse(digit.ToString()) * multiplier).Sum();
            var checkSum = 11 - (sum % 11);

            var expectedCheckDigit = CheckDigitsForCheckSum[checkSum - 1];
            var checkDigit = match.Groups[4].Value[0];

            if (expectedCheckDigit != checkDigit)
            {
                // FIXME {all, 22.01.2014}: требуется локализация
                message = "Invalid checksum";
                return true;
            }

            message = null;
            return false;
        }
    }
}

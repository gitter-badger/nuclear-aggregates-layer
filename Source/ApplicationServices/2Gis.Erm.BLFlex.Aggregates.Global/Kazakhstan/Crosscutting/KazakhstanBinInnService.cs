using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.Resources.Server;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Kazakhstan.Crosscutting
{
    public class KazakhstanBinInnService : ICheckInnService, IKazakhstanAdapted
    {
        private static readonly int[] WeightCoefficients = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
        private static readonly int[] WeightCoefficients10 = { 3, 4, 5, 6, 7, 8, 9, 10, 11, 1, 2 };

        public bool TryGetErrorMessage(string inn, out string message)
        {
            message = null;

            long _;
            if (inn == null || inn.Length != 12 || !long.TryParse(inn, out _))
            {
                message = ResPlatform.InvalidFieldFormat;
                return true;
            }

            var innDigits = inn.Select(c => int.Parse(c.ToString())).ToArray();

            var checksum = CalculateSum(innDigits, WeightCoefficients);
            if (checksum == 10)
            {
                checksum = CalculateSum(innDigits, WeightCoefficients10);
            }

            if (innDigits[11] != checksum)
            {
                message = ResPlatform.InvalidFieldValue;
                return true;
            }

            return false;
        }

        private static int CalculateSum(int[] innDigits, int[] weightCoefficients)
        {
            var sum = 0;
            for (var i = 0; i < 11; i++)
            {
                sum += innDigits[i] * weightCoefficients[i];
            }

            return sum % 11;
        }
    }
}
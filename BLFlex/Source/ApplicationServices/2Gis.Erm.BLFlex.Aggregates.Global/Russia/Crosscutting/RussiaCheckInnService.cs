using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Russia.Crosscutting
{
    // TODO {all, 21.10.2013}: существует несколько реализаций Inn сервис, в зависимости от целевой бизнес модели
    public sealed class RussiaCheckInnService : ICheckInnService, IRussiaAdapted
    {
        public bool TryGetErrorMessage(string inn, out string message)
        {
            var weightCoefficients10 = new sbyte[] { 2, 4, 10, 3, 5, 9, 4, 6, 8, 0 };
            var weightCoefficients11 = new sbyte[] { 7, 2, 4, 10, 3, 5, 9, 4, 6, 8, 0, 0 };
            var weightCoefficients12 = new sbyte[] { 3, 7, 2, 4, 10, 3, 5, 9, 4, 6, 8, 0 };

            long parseResult;
            if (!long.TryParse(inn, out parseResult))
            {
                message = BLResources.InnMustContainNumbersOnly;
                return true;
            }

            sbyte count = 0;
            switch (inn.Length)
            {
                case 10:
                    var sum = inn.Sum(digit => Convert.ToSByte(digit.ToString()) * weightCoefficients10[count++]);
                    var checkNumber = Mod(sum);

                    if (checkNumber != Convert.ToInt32(inn[9].ToString()))
                    {
                        message = BLResources.EnteredInnIsNotCorrect;
                        return true;
                    }
                    break;
                case 12:
                    var sum1 = inn.Sum(digit => Convert.ToSByte(digit.ToString()) * weightCoefficients11[count++]);
                    var checkNumber1 = Mod(sum1);

                    count = 0;
                    var sum2 = inn.Sum(digit => Convert.ToSByte(digit.ToString()) * weightCoefficients12[count++]);
                    var checkNumber2 = Mod(sum2);

                    if (checkNumber1 != Convert.ToInt32(inn[10].ToString()) || checkNumber2 != Convert.ToInt32(inn[11].ToString()))
                    {
                        message = BLResources.EnteredInnIsNotCorrect;
                        return true;
                    }
                    break;
                default:
                    message = BLResources.InnMustConsistOf10Or12Digits;
                    return true;
            }

            message = null;
            return false;
        }

        private static int Mod(int sum)
        {
            var checkNumber = sum % 11;
            checkNumber %= 10;
            return checkNumber;
        }
    }
}

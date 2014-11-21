// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles
// ReSharper restore CheckNamespace
{
    public sealed class NumberFormatInfoDto
    {
        public int CurrencyDecimalDigits { get; set; }

        public string CurrencyDecimalSeparator { get; set; }

        public string CurrencyGroupSeparator { get; set; }

        public int[] CurrencyGroupSizes { get; set; }

        public string CurrencyNegativePattern { get; set; }

        public string CurrencyPositivePattern { get; set; }

        public string CurrencySymbol { get; set; }

        public string NegativeInfinitySymbol { get; set; }

        public string NegativeSign { get; set; }

        public int NumberDecimalDigits { get; set; }

        public string NumberDecimalSeparator { get; set; }

        public string NumberGroupSeparator { get; set; }

        public int[] NumberGroupSizes { get; set; }

        public string NumberNegativePattern { get; set; }
            
        public string PositiveInfinitySymbol { get; set; }

        public string PositiveSign { get; set; }
    }
}
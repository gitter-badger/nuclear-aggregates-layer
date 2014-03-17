using System.Globalization;
using System.Text;

namespace DoubleGis.Erm.Platform.Common.Utils
{
    public static class EncodingUtils
    {
        public static Encoding ToDefaultAnsiEncoding(this CultureInfo cultureInfo)
        {
            int codePage = cultureInfo.TextInfo.ANSICodePage;
            return codePage.Equals(0) ? Encoding.UTF8 : Encoding.GetEncoding(codePage);
        }
    }
}

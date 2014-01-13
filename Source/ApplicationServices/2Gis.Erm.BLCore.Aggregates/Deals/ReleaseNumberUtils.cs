using System;

using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals
{
    public static class ReleaseNumberUtils
    {
        private static readonly DateTime NskFirstReleaseDate = new DateTime(1998, 10, 1);

        public static int ToAbsoluteReleaseNumber(this DateTime date)
        {
            return date.MonthDifference(NskFirstReleaseDate) + 1;
        }

        public static DateTime AbsoluteReleaseNumberToMonth(int monthNumber)
        {
            return NskFirstReleaseDate.AddMonths(monthNumber - 1);
        }
    }
}

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Common.Utils;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting
{
    public sealed class MonthPeriodValidationService : IMonthPeriodValidationService
    {
        public bool IsValid(TimePeriod period, out string report)
        {
            report = null;

            if (period.Start != period.Start.GetFirstDateOfMonth())
            {
                report = BLResources.StartPeriodDateMustBeFirstMonthDay;
                return false;
            }

            if (period.End != period.End.GetEndPeriodOfThisMonth())
            {
                report = BLResources.EndPeriodDateMustBeLastMonthDay;
                return false;
            }

            if (period.Start.Year != period.End.Year ||
                period.Start.Month - period.End.Month != 0)
            {
                report = BLResources.PeriodDateMustBeEqualToMonth;
                return false;
            }

            return true;
        }
    }
}
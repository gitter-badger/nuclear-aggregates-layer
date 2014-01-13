using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Common.Crosscutting;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting
{
    public interface ICheckOperationPeriodService : IInvariantSafeCrosscuttingService
    {
        bool IsOperationPeriodValid(TimePeriod period, out string report);
    }
}

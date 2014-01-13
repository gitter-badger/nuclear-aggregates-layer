using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public interface IEnsureOrderExportedStrategy
    {
        bool IsExported(long releaseId, long organizationUnitId, int organizationUnitDgppId, TimePeriod period, bool isBeta);
    }
}
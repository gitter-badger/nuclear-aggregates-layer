using System.IO;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public interface IPublishOrdersForReleaseToFileStorage
    {
        void Publish(long organizationUnitId, int organizationUnitDgppId, TimePeriod period, bool isBeta, Stream ordersStream);
    }
}
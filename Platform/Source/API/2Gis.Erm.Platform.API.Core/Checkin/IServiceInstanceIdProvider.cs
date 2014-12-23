using System;

namespace DoubleGis.Erm.Platform.API.Core.Checkin
{
    public interface IServiceInstanceIdProvider
    {
        Guid GetInstanceId(TimeSpan timeout);
        bool TryGetInstanceId(TimeSpan timeout, out Guid id);
    }
}
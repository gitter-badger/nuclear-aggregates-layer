using System;

namespace DoubleGis.Erm.Platform.API.Core.Checkin
{
    public interface IServiceInstanceCheckinService
    {
        void Start();
        void Stop();
        event EventHandler<UnhandledExceptionEventArgs> Faulted;
    }
}
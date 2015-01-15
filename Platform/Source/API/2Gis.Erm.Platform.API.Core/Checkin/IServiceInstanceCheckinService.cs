using System;

namespace DoubleGis.Erm.Platform.API.Core.Checkin
{
    public interface IServiceInstanceCheckinService
    {
        event EventHandler<UnhandledExceptionEventArgs> Faulted;
        void Start();
        void Stop();
    }
}
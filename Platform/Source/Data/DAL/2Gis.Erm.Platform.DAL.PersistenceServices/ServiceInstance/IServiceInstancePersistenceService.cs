using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices.ServiceInstance
{
    public interface IServiceInstancePersistenceService
    {
        void Add(ServiceInstanceDto serviceInstance);
        IEnumerable<RunningServiceInstanceDto> GetRunningInstances();
        void Checkin(Guid instanceId, DateTimeOffset now);
        void ReportNotRunning(IEnumerable<Guid> ids, bool isSelfReport);
        bool IsRunning(Guid instanceId);
    }
}
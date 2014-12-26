using System;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices.ServiceInstance
{
    public class RunningServiceInstanceDto
    {
        public Guid Id { get; set; }
        public DateTimeOffset LastCheckinTime { get; set; }
        public TimeSpan CheckinInterval { get; set; }
        public TimeSpan TimeSafetyOffset { get; set; }
    }
}
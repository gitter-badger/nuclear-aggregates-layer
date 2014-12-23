using System;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices.ServiceInstance
{
    public class ServiceInstanceDto
    {
        public Guid Id { get; set; }

        public string Environment { get; set; }
        public string EntryPoint { get; set; }
        public string ServiceName { get; set; }
        public string Host { get; set; }

        public DateTimeOffset FirstCheckinTime { get; set; }
        public DateTimeOffset LastCheckinTime { get; set; }
        public TimeSpan CheckinInterval { get; set; }

        public bool IsRunning { get; set; }
        public bool IsSelfReport { get; set; }
    }
}
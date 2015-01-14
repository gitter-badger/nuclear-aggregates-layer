using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.AdoNet;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices.ServiceInstance
{
    public class ServiceInstancePersistenceService : IServiceInstancePersistenceService
    {
        private readonly IDatabaseCaller _databaseCaller;

        public ServiceInstancePersistenceService(IDatabaseCaller databaseCaller)
        {
            _databaseCaller = databaseCaller;
        }

        public void Add(ServiceInstanceDto serviceInstance)
        {
            _databaseCaller.ExecuteRawSql(@"INSERT INTO [Metadata].[ServiceInstances]
                                                       ([Id]
                                                       ,[Environment]
                                                       ,[EntryPoint]
                                                       ,[ServiceName]
                                                       ,[Host]
                                                       ,[FirstCheckinTime]
                                                       ,[LastCheckinTime]
                                                       ,[CheckinIntervalMs]
                                                       ,[TimeSafetyOffsetMs]
                                                       ,[IsRunning]
                                                       ,[IsSelfReport])
                                                 VALUES
                                                       (@Id
                                                       ,@Environment
                                                       ,@EntryPoint
                                                       ,@ServiceName
                                                       ,@Host
                                                       ,@FirstCheckinTime
                                                       ,@LastCheckinTime
                                                       ,@CheckinIntervalMs
                                                       ,@TimeSafetyOffsetMs
                                                       ,@IsRunning
                                                       ,@IsSelfReport)",
                                          new ServiceInstance
                                              {
                                                  Id = serviceInstance.Id,
                                                  Environment = serviceInstance.Environment,
                                                  EntryPoint = serviceInstance.EntryPoint,
                                                  ServiceName = serviceInstance.ServiceName,
                                                  Host = serviceInstance.Host,
                                                  FirstCheckinTime = serviceInstance.FirstCheckinTime,
                                                  LastCheckinTime = serviceInstance.LastCheckinTime,
                                                  CheckinIntervalMs = (int)serviceInstance.CheckinInterval.TotalMilliseconds,
                                                  TimeSafetyOffsetMs = (int)serviceInstance.TimeSafetyOffset.TotalMilliseconds,
                                                  IsRunning = serviceInstance.IsRunning,
                                                  IsSelfReport = serviceInstance.IsSelfReport
                                              });
        }

        public IEnumerable<RunningServiceInstanceDto> GetRunningInstances()
        {
            return _databaseCaller.QueryRawSql<RunningServiceInstance>(@"SELECT [Id]
                                                                          ,[LastCheckinTime]
                                                                          ,[CheckinIntervalMs]
                                                                          ,[TimeSafetyOffsetMs]
                                                                FROM [Metadata].[ServiceInstances] WHERE [IsRunning] = 1")
                                  .Select(x => new RunningServiceInstanceDto
                                                   {
                                                       Id = x.Id,
                                                       LastCheckinTime = x.LastCheckinTime,
                                                       CheckinInterval = TimeSpan.FromMilliseconds(x.CheckinIntervalMs),
                                                       TimeSafetyOffset = TimeSpan.FromMilliseconds(x.TimeSafetyOffsetMs)
                                                   });
        }

        public void Checkin(Guid instanceId, DateTimeOffset now)
        {
            _databaseCaller.ExecuteRawSql("UPDATE [Metadata].[ServiceInstances] SET [LastCheckinTime] = @LastCheckinTime WHERE [Id] = @Id",
                                          new
                                              {
                                                  LastCheckinTime = now,
                                                  Id = instanceId
                                              });
        }

        public void ReportNotRunning(IEnumerable<Guid> ids, bool isSelfReport)
        {
            _databaseCaller.ExecuteRawSql("UPDATE [Metadata].[ServiceInstances] SET [IsRunning] = 0, [IsSelfReport] = @IsSelfReport WHERE [Id] IN @Ids",
                                          new { IsSelfReport = isSelfReport, Ids = ids });
        }

        public bool IsRunning(Guid instanceId)
        {
            return _databaseCaller.QueryRawSql<bool>(@"SELECT [IsRunning] FROM [Metadata].[ServiceInstances] WHERE [Id] = @Id", new { Id = instanceId }).Single();
        }

        #region nested

        private class RunningServiceInstance
        {
            public Guid Id { get; set; }
            public int CheckinIntervalMs { get; set; }
            public DateTimeOffset LastCheckinTime { get; set; }
            public int TimeSafetyOffsetMs { get; set; }
        }

        #endregion
    }
}
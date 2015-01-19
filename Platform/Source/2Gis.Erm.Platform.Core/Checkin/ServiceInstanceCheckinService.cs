using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Core.Checkin;
using DoubleGis.Erm.Platform.API.Core.Locking;
using DoubleGis.Erm.Platform.API.Core.Settings.Checkin;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.PersistenceServices.ServiceInstance;

namespace DoubleGis.Erm.Platform.Core.Checkin
{
    public sealed class ServiceInstanceCheckinService : IServiceInstanceCheckinService, IServiceInstanceIdProvider, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _workerTask;
        private readonly AutoResetEvent _asyncWorkerSignal;
        private readonly ManualResetEventSlim _instanceIdAcquiredSignal;

        private readonly IApplicationLocksService _applicationLocksService;
        private readonly IServiceInstancePersistenceService _serviceInstancePersistenceService;
        private readonly IEnvironmentSettings _environmentSettings;
        private readonly IServiceInstanceCheckinSettings _serviceInstanceCheckinSettings;
        private readonly ICommonLog _logger;

        private readonly string _serviceName;
        private readonly string _host;

        private Guid? _instanceId;
        private bool _disposed;

        public ServiceInstanceCheckinService(IServiceInstancePersistenceService serviceInstancePersistenceService, 
                                             IApplicationLocksService applicationLocksService, 
                                             IEnvironmentSettings environmentSettings, 
                                             IServiceInstanceCheckinSettings serviceInstanceCheckinSettings, 
                                             ICommonLog logger, 
                                             string serviceName)
        {
            _serviceInstancePersistenceService = serviceInstancePersistenceService;
            _logger = logger;
            _applicationLocksService = applicationLocksService;
            _environmentSettings = environmentSettings;
            _serviceInstanceCheckinSettings = serviceInstanceCheckinSettings;
            _serviceName = serviceName;

            _cancellationTokenSource = new CancellationTokenSource();
            _workerTask = new Task(() => Worker(_cancellationTokenSource.Token), _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
            _workerTask.ContinueWith(OnFaulted, TaskContinuationOptions.OnlyOnFaulted);

            _asyncWorkerSignal = new AutoResetEvent(false);
            _instanceIdAcquiredSignal = new ManualResetEventSlim(false);
            _host = GetHostName();
        }

        public event EventHandler<UnhandledExceptionEventArgs> Faulted = delegate { };

        public void Start()
        {
            ThrowIfDisposed();
            _logger.InfoEx("Starting service instance checkin service");
            _workerTask.Start();
            _logger.InfoFormatEx("Service instance checkin service successfully started");
        }

        public void Stop()
        {
            ThrowIfDisposed();
            _logger.InfoFormatEx("Stopping service instance checkin service. Id = {0}", _instanceId);
            _cancellationTokenSource.Cancel();
            _asyncWorkerSignal.Set();

            try
            {
                if (!_workerTask.IsCompleted)
                {
                    _workerTask.Wait();
                }
            }
            finally
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    // ReSharper disable once PossibleInvalidOperationException
                    _serviceInstancePersistenceService.ReportNotRunning(new[] { _instanceId.Value }, true);
                    transaction.Complete();
                }

                _asyncWorkerSignal.Close();
            }

            _logger.InfoFormatEx("Service instance checkin service successfully stopped. Id = {0}", _instanceId);
        }

        public Guid GetInstanceId(TimeSpan timeout)
        {
            ThrowIfDisposed();
            Guid id;
            if (!TryGetInstanceId(timeout, out id))
            {
                throw new TimeoutException();
            }

            return id;
        }

        public bool TryGetInstanceId(TimeSpan timeout, out Guid id)
        {
            ThrowIfDisposed();
            _instanceIdAcquiredSignal.Wait(timeout);
            if (_instanceId != null)
            {
                id = _instanceId.Value;
                return true;
            }

            id = Guid.Empty;
            return false;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _cancellationTokenSource.Dispose();
            _workerTask.Dispose();
            _asyncWorkerSignal.Dispose();
            _instanceIdAcquiredSignal.Dispose();

            _disposed = true;
        }

        private static bool IsFailed(DateTimeOffset startTime, DateTimeOffset lastCheckinTime, TimeSpan checkinInterval, TimeSpan timeSafetyOffset)
        {
            return startTime.Subtract(lastCheckinTime) >= checkinInterval.Add(timeSafetyOffset);
        }

        private static string GetHostName()
        {
            return Dns.GetHostEntry(string.Empty).HostName;
        }

        private void Worker(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var startTime = DateTimeOffset.UtcNow;
                if (_instanceId == null)
                {
                    _instanceId = FirstCheckin(startTime);
                    _instanceIdAcquiredSignal.Set();
                }
                else
                {
                    Checkin(startTime);
                }

                ILockingScope serviceInstancesAccessLock;
                if (_applicationLocksService.TryAcquire(LockName.ReportFailedInstances, LockOwner.Session, out serviceInstancesAccessLock))
                {
                    using (serviceInstancesAccessLock)
                    {
                        ReportNotRunningInstances(startTime);
                        serviceInstancesAccessLock.Release();
                    }
                }

                var timeElapsed = DateTimeOffset.UtcNow.Subtract(startTime);
                var timeToSleep = _serviceInstanceCheckinSettings.CheckinInterval.Subtract(timeElapsed);
                if (timeToSleep > TimeSpan.Zero)
                {
                    _asyncWorkerSignal.WaitOne(timeToSleep);
                }
            }
        }

        private void Checkin(DateTimeOffset startTime)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                // ReSharper disable once PossibleInvalidOperationException
                if (!_serviceInstancePersistenceService.IsRunning(_instanceId.Value))
                {
                    throw new InvalidOperationException(string.Format("Current instance has been considered as not running. Id = {0}", _instanceId));
                }

                _serviceInstancePersistenceService.Checkin(_instanceId.Value, startTime);
                transaction.Complete();
            }
        }

        private void ReportNotRunningInstances(DateTimeOffset startTime)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                var runningInstances = _serviceInstancePersistenceService.GetRunningInstances();

                var failedInstances = new List<Guid>();
                foreach (var runningInstance in runningInstances)
                {
                    if (runningInstance.Id != _instanceId && IsFailed(startTime, runningInstance.LastCheckinTime, runningInstance.CheckinInterval, runningInstance.TimeSafetyOffset))
                    {
                        failedInstances.Add(runningInstance.Id);
                    }
                }

                if (failedInstances.Any())
                {
                    _serviceInstancePersistenceService.ReportNotRunning(failedInstances, false);
                }

                transaction.Complete();
            }
        }

        private Guid FirstCheckin(DateTimeOffset startTime)
        {
            var id = Guid.NewGuid();
            var serviceInstance = new ServiceInstanceDto
                                      {
                                          Id = id, 
                                          Environment = _environmentSettings.EnvironmentName, 
                                          EntryPoint = _environmentSettings.EntryPointName, 
                                          Host = _host, 
                                          ServiceName = _serviceName, 
                                          FirstCheckinTime = startTime, 
                                          LastCheckinTime = startTime, 
                                          CheckinInterval = _serviceInstanceCheckinSettings.CheckinInterval, 
                                          TimeSafetyOffset = _serviceInstanceCheckinSettings.CheckinTimeSafetyOffset, 
                                          IsRunning = true, 
                                          IsSelfReport = true
                                      };

            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                _logger.InfoFormatEx("Starting first checkin. Id = {0}", id);
                _serviceInstancePersistenceService.Add(serviceInstance);
                _logger.InfoFormatEx("Successfully checked in. Id = {0}", id);
                transaction.Complete();
            }

            return id;
        }

        private void OnFaulted(Task task)
        {
            if (task.Exception != null)
            {
                task.Exception.Flatten().Handle(ex =>
                                                    {
                                                        _logger.FatalFormatEx(ex, "Service instance checkin service is faulted. Id = {0}", _instanceId);
                                                        Faulted(this, new UnhandledExceptionEventArgs(ex, false));
                                                        return true;
                                                    });
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}
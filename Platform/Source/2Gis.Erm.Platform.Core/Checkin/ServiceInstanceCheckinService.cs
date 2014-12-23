using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using DoubleGis.Erm.Platform.API.Core.Checkin;
using DoubleGis.Erm.Platform.API.Core.Locking;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.PersistenceServices.ServiceInstance;
using DoubleGis.Erm.Platform.DAL.Transactions;

namespace DoubleGis.Erm.Platform.Core.Checkin
{
    public sealed class ServiceInstanceCheckinService : IServiceInstanceCheckinService, IServiceInstanceIdProvider, IDisposable
    {
        // FIXME {a.tukaev, 19.12.2014}: Вытащить в настройки
        private readonly TimeSpan _checkinInterval = TimeSpan.FromSeconds(5);
        private readonly TimeSpan _timeSafetyOffset = TimeSpan.FromSeconds(5);

        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _workerTask;
        private readonly AutoResetEvent _asyncWorkerSignal;
        private readonly ManualResetEventSlim _instanceIdAcquiredSignal;
        private Guid? _instanceId;
        private readonly string _host;


        private readonly IApplicationLocksService _applicationLocksService;
        private readonly IServiceInstancePersistenceService _serviceInstancePersistenceService;
        private readonly ICommonLog _logger;
        private readonly IEnvironmentSettings _environmentSettings;
        private readonly string _serviceName;

        private bool _disposed;

        public ServiceInstanceCheckinService(IServiceInstancePersistenceService serviceInstancePersistenceService,
                                             ICommonLog logger,
                                             IApplicationLocksService applicationLocksService, 
                                             IEnvironmentSettings environmentSettings,
                                             string serviceName)
        {
            _serviceInstancePersistenceService = serviceInstancePersistenceService;
            _logger = logger;
            _applicationLocksService = applicationLocksService;
            _environmentSettings = environmentSettings;
            _serviceName = serviceName;

            _cancellationTokenSource = new CancellationTokenSource();
            _workerTask = new Task(() => Worker(_cancellationTokenSource.Token), _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
            _asyncWorkerSignal = new AutoResetEvent(false);
            _instanceIdAcquiredSignal = new ManualResetEventSlim(false);
            _host = ResolveHostName();
        }

        public void Start()
        {
            ThrowIfDisposed();
            _logger.InfoEx("Starting service instance checkin service...");
            _workerTask.Start();
            _logger.InfoEx("Service instance checkin service successfully started");
        }

        public void Stop()
        {
            ThrowIfDisposed();
            _logger.InfoEx("Stopping service instance checkin service...");
            _cancellationTokenSource.Cancel();
            _asyncWorkerSignal.Set();
            _workerTask.Wait();

            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, DefaultTransactionOptions.Default))
            {
                // ReSharper disable once PossibleInvalidOperationException
                _serviceInstancePersistenceService.ReportNotRunning(new[] { _instanceId.Value }, true);
                transaction.Complete();
            }

            _asyncWorkerSignal.Close();
            _logger.InfoEx("Service instance checkin service successfully stopped");
        }

        private void Worker(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var startTime = DateTimeOffset.UtcNow;
                if (_instanceId == null)
                {
                    _instanceId = FirstCheckin();
                    _instanceIdAcquiredSignal.Set();
                }
                else
                {
                    Checkin();
                }

                ILockingScope serviceInstancesAccessLock;
                if (_applicationLocksService.TryAcquire(LockName.ReportFailedInstances, out serviceInstancesAccessLock))
                {
                    using (serviceInstancesAccessLock)
                    {
                        ReportNotRunningInstances();
                        serviceInstancesAccessLock.Release();
                    }
                }

                var timeElapsed = DateTimeOffset.UtcNow.Subtract(startTime);
                var timeToSleep = _checkinInterval.Subtract(timeElapsed);
                if (timeToSleep > TimeSpan.Zero)
                {
                    _asyncWorkerSignal.WaitOne(timeToSleep);
                }
            }
        }

        private void Checkin()
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, DefaultTransactionOptions.Default))
            {
                // ReSharper disable once PossibleInvalidOperationException
                _serviceInstancePersistenceService.Checkin(_instanceId.Value, DateTimeOffset.UtcNow);

                transaction.Complete();
            }
        }

        private void ReportNotRunningInstances()
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, DefaultTransactionOptions.Default))
            {
                var now = DateTimeOffset.UtcNow;
                var runningInstances = _serviceInstancePersistenceService.GetRunningInstances();

                bool currentInstanceFound = false;
                var failedInstances = new List<Guid>();
                foreach (var runningInstance in runningInstances)
                {
                    if (runningInstance.Id == _instanceId)
                    {
                        currentInstanceFound = true;
                    }
                    else if (IsFailed(now, runningInstance.LastCheckinTime, runningInstance.CheckinInterval))
                    {
                        failedInstances.Add(runningInstance.Id);
                    }
                }

                if (!currentInstanceFound)
                {
                    throw new InvalidOperationException("Current instance has been considered as not running");
                }

                if (failedInstances.Contains(_instanceId.Value))
                {
                    _logger.WarnEx("Current instance has been considered as not running by itself");
                }

                _serviceInstancePersistenceService.ReportNotRunning(failedInstances, false);
                transaction.Complete();
            }
        }

        private bool IsFailed(DateTimeOffset now, DateTimeOffset lastCheckinTime, TimeSpan checkinInterval)
        {
            return now.Subtract(lastCheckinTime) >= checkinInterval.Add(_timeSafetyOffset);
        }

        private Guid FirstCheckin()
        {
            var id = Guid.NewGuid();
            var serviceInstance = new ServiceInstanceDto
                                      {
                                          Id = id,
                                          Environment = _environmentSettings.EnvironmentName,
                                          EntryPoint = _environmentSettings.EntryPointName,
                                          Host = _host,
                                          ServiceName = _serviceName,
                                          FirstCheckinTime = DateTimeOffset.UtcNow,
                                          LastCheckinTime = DateTimeOffset.UtcNow,
                                          CheckinInterval = _checkinInterval,
                                          IsRunning = true,
                                          IsSelfReport = true
                                      };

            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, DefaultTransactionOptions.Default))
            {
                _serviceInstancePersistenceService.Add(serviceInstance);
                transaction.Complete();
            }
            
            return id;
        }

        private static string ResolveHostName()
        {
            try
            {
                return Dns.GetHostName();
            }
            catch (SocketException)
            {
                return "[UNRESOLVED]";
            }
        }

        public Guid GetInstanceId(TimeSpan timeout)
        {
            ThrowIfDisposed();
            Guid id;
            if (!TryGetInstanceId(timeout, out id))
            {
                return id;
            }

            throw new TimeoutException();
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

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}
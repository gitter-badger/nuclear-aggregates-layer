using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.TaskService.Settings;

using Nuclear.Tracing.API;

using Quartz;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl.AdoJobStore.Common;
using Quartz.Impl.Matchers;
using Quartz.Plugin.Xml;
using Quartz.Simpl;
using Quartz.Spi;
using Quartz.Util;

namespace DoubleGis.Erm.Platform.TaskService.Schedulers
{
    public sealed class SchedulerManager : ISchedulerManager
    {
        private const string DataSourceName = "SchedulerData";

        private readonly ITracer _logger;
        private readonly ITaskServiceProcessingSettings _processingSettings;
        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly IJobFactory _jobFactory;
        private readonly IInstanceIdGenerator _instanceIdGenerator = new SimpleInstanceIdGenerator();

        public SchedulerManager(
            ITaskServiceProcessingSettings processingSettings,
            IConnectionStringSettings connectionStringSettings,
            IJobFactory jobFactory,
            ITracer logger)
        {
            _logger = logger;
            _processingSettings = processingSettings;
            _connectionStringSettings = connectionStringSettings;
            _jobFactory = jobFactory;

            _logger.InfoFormat("Версия сервиса: {0} ", ThisAssembly.SemanticVersion);
        }

        public void Start()
        {
            try
            {
                var threadPool = new SimpleThreadPool(_processingSettings.MaxWorkingThreads, ThreadPriority.Normal);
                threadPool.Initialize();

                var directory = AppDomain.CurrentDomain.BaseDirectory;
                var files = Directory.GetFiles(directory, "quartz*.config").OrderBy(x => x.Length);
                var fileNames = string.Join(",", files);

                var jobInitializationPlugin = new ConfigFileProcessorPlugin
                                                  {
                                                      FileNames = fileNames,
                                                      FailOnFileNotFound = true,
                                                      ScanInterval = QuartzConfigFileScanInterval.DisableScanning
                                                  };

                var instanceId = _instanceIdGenerator.GenerateInstanceId();
                DirectSchedulerFactory.Instance.CreateScheduler(_processingSettings.SchedulerName,
                                                                instanceId,
                                                                threadPool,
                                                                new DefaultThreadExecutor(),
                                                                CreateJobStore(instanceId),
                                                                new Dictionary<string, ISchedulerPlugin>
                                                                    {
                                                                        { _processingSettings.SchedulerName, jobInitializationPlugin }
                                                                    },
                                                                TimeSpan.Zero,
                                                                1,
                                                                TimeSpan.Zero);

                var scheduler = SchedulerRepository.Instance.Lookup(_processingSettings.SchedulerName);

                scheduler.JobFactory = _jobFactory;
                scheduler.Start();

                _logger.Debug("Сервис успешно запущен");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Произошла ошибка при инициализации сервиса");
            }
        }

        public void Stop()
        {
            try
            {
                var scheduler = SchedulerRepository.Instance.Lookup(_processingSettings.SchedulerName);
                if (scheduler == null)
                {
                    throw new ApplicationException("Не найден scheduler");
                }

                const string InterruptableJobIndicatorSuffix = "Interruptable";

                // COMMENT {all, 11.07.2014}: scheduler.GetCurrentlyExecutingJobs() is not cluster aware, но потенциально можно впилить, что-нибудь вида scheduler.GetCurrentlyExecutingJobs().Select(context =>  context.JobInstance).OfType<IInterruptableJob>()
                var matcher = GroupMatcher<JobKey>.GroupEndsWith(InterruptableJobIndicatorSuffix);
                foreach (var jobKey in scheduler.GetJobKeys(matcher))
                {
                    try
                    {
                        scheduler.Interrupt(jobKey);
                    }
                    catch (UnableToInterruptJobException ex)
                    {
                        const string MsgTemplate = "Can't interrupt job with key {0} and group {1}. " +
                                                   "Only jobs that are implements {2} can be in group with \"{3}\" suffix. " +
                                                   "Check quartz configuration.";
                        _logger.ErrorFormat(ex, MsgTemplate, jobKey.Name, jobKey.Group, typeof(IInterruptableJob), InterruptableJobIndicatorSuffix);
                    }
                }

                scheduler.Shutdown(true);
                SchedulerRepository.Instance.Remove(_processingSettings.SchedulerName);

                _logger.Debug("Сервис успешно остановлен");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Произошла ошибка при остановке сервиса");
            }
        }

        private IJobStore CreateJobStore(string instanceId)
        {
            switch (_processingSettings.JobStoreType)
            {
                case JobStoreType.RAM:
                {
                    return new RAMJobStore();
                }
                case JobStoreType.TX:
                {
                    var jobStoreConnectionString = _connectionStringSettings.GetConnectionStringSettings(ConnectionStringName.QuartzJobStore);
                    DBConnectionManager.Instance.AddConnectionProvider(DataSourceName,
                                                                       new DbProvider(jobStoreConnectionString.ProviderName,
                                                                                      jobStoreConnectionString.ConnectionString));

                    return new JobStoreTX
                               {
                                   DataSource = DataSourceName,
                                   TablePrefix = string.Empty,
                                   InstanceId = instanceId,
                                   InstanceName = _processingSettings.SchedulerName,
                                   DriverDelegateType = typeof(SqlServerDelegate).AssemblyQualifiedName,
                                   Clustered = true
                               };
                }
                default:
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Класс нужен для очистки всех сохраненных в persistence данных о job'ах и триггерах непосредственно перед чтением quartz.config.
        /// Т.о., каждый новый запущенный instance будет заменять существующие данные для всего кластера прочитанными из quartz.config.
        /// </summary>
        private class ConfigFileProcessorPlugin : XMLSchedulingDataProcessorPlugin
        {
            public override void Initialize(string pluginName, IScheduler sched)
            {
                sched.Clear();
                base.Initialize(pluginName, sched);
            }
        }
    }
}

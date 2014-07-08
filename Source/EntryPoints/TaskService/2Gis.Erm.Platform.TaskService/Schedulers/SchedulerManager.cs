using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using DoubleGis.Erm.Platform.Common;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.TaskService.Settings;

using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Plugin.Xml;
using Quartz.Simpl;
using Quartz.Spi;

namespace DoubleGis.Erm.Platform.TaskService.Schedulers
{
    public sealed class SchedulerManager : ISchedulerManager
    {
        private const string SchedulerName = "SCHEDULER";

        private readonly ICommonLog _logger;
        private readonly ITaskServiceProcesingSettings _processingSettings;
        private readonly IJobFactory _jobFactory;

        public SchedulerManager(
            ITaskServiceProcesingSettings processingSettings, 
            IJobFactory jobFactory,
            ICommonLog logger)
        {
            _logger = logger;
            _processingSettings = processingSettings;
            _jobFactory = jobFactory;

            _logger.InfoFormatEx("Версия сервиса: {0} ", SolutionInfo.FileVersion);
        }

        public void Start()
        {
            try
            {
                var threadPool = new SimpleThreadPool(_processingSettings.MaxWorkingThreads, ThreadPriority.Normal);
                threadPool.Initialize();

                var jobInitializationPlugin = new XMLSchedulingDataProcessorPlugin
                {
                    FileNames = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "quartz.config"),
                    FailOnFileNotFound = true,
                };

                DirectSchedulerFactory.Instance.CreateScheduler(
                    SchedulerName,
                    "AUTO",
                    threadPool,
                    new DefaultThreadExecutor(),
                    new RAMJobStore(),
                    new Dictionary<string, ISchedulerPlugin> { { SchedulerName, jobInitializationPlugin } },
                    TimeSpan.Zero,
                    1,
                    TimeSpan.Zero);

                var scheduler = SchedulerRepository.Instance.Lookup(SchedulerName);
                
                scheduler.JobFactory = _jobFactory;
                scheduler.Start();

                _logger.DebugEx("Сервис успешно запущен");
            }
            catch (Exception ex)
            {
                _logger.ErrorEx(ex, "Произошла ошибка при инициализации сервиса");
            }
        }

        public void Stop()
        {
            try
            {
                var scheduler = SchedulerRepository.Instance.Lookup(SchedulerName);
                if (scheduler == null)
                {
                    throw new ApplicationException("Не найден scheduler");
                }

                const string InterruptibleJobIndicatorSuffix = "Interruptable";
                var matcher = GroupMatcher<JobKey>.GroupEndsWith(InterruptibleJobIndicatorSuffix);
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
                        _logger.ErrorFormatEx(ex, MsgTemplate, jobKey.Name,  jobKey.Group, typeof(IInterruptableJob), InterruptibleJobIndicatorSuffix);
                    }
                }

                scheduler.Shutdown(true);
                SchedulerRepository.Instance.Remove(SchedulerName);

                _logger.DebugEx("Сервис успешно остановлен");
            }
            catch (Exception ex)
            {
                _logger.ErrorEx(ex, "Произошла ошибка при остановке сервиса");
            }
        }
    }
}

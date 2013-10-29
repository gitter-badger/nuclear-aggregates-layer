using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Logging;

using Microsoft.Practices.Unity;

using Quartz;
using Quartz.Simpl;
using Quartz.Spi;

namespace DoubleGis.Erm.Platform.TaskService.Schedulers
{
    public sealed class JobFactory : PropertySettingJobFactory
    {
        private readonly Dictionary<IJob, IUnityContainer> _containerMap = new Dictionary<IJob, IUnityContainer>();

        private readonly IUnityContainer _container;
        private readonly ICommonLog _logger;

        public JobFactory(IUnityContainer container, ICommonLog logger)
        {
            _container = container;
            _logger = logger;
        }

        public override IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                var jobDataMap = new JobDataMap();
                jobDataMap.PutAll(scheduler.Context);
                jobDataMap.PutAll(bundle.JobDetail.JobDataMap);
                jobDataMap.PutAll(bundle.Trigger.JobDataMap);

                var childContainer = _container.CreateChildContainer();
                var job = (IJob)childContainer.Resolve(bundle.JobDetail.JobType);
                SetObjectProperties(job, jobDataMap);

                _containerMap.Add(job, childContainer);
                
                _logger.DebugEx(string.Format("Создание задачи [{0}]", bundle.JobDetail.JobType));
                return job;
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(ex, "Произошла ошибка при выполнении задачи [{0}]", bundle.JobDetail.Description);
                throw new SchedulerException(ex);
            }
        }

        public override void ReturnJob(IJob job)
        {
            var childContainer = _containerMap[job];
            _containerMap.Remove(job);

            childContainer.Dispose();
        }
    }
}
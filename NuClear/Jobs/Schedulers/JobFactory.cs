using System;
using System.Collections.Generic;

using Microsoft.Practices.Unity;

using NuClear.Tracing.API;

using Quartz;
using Quartz.Simpl;
using Quartz.Spi;

namespace NuClear.Jobs.Schedulers
{
    public sealed class JobFactory : PropertySettingJobFactory
    {
        private readonly object _containerMapSync = new object();
        private readonly Dictionary<IJob, IUnityContainer> _containerMap = new Dictionary<IJob, IUnityContainer>();

        private readonly IUnityContainer _container;
        private readonly ITracer _tracer;

        public JobFactory(IUnityContainer container, ITracer tracer)
        {
            _container = container;
            _tracer = tracer;
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

                lock (_containerMapSync)
                {
                    _containerMap.Add(job, childContainer);
                }
                
                _tracer.Debug(string.Format("Создание задачи [{0}]", bundle.JobDetail.JobType));
                return job;
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Произошла ошибка при выполнении задачи [{0}]", bundle.JobDetail.Description);
                throw new SchedulerException(ex);
            }
        }

        public override void ReturnJob(IJob job)
        {
            IUnityContainer childContainer;

            lock (_containerMapSync)
            {
                childContainer = _containerMap[job];
                _containerMap.Remove(job);
            }

            childContainer.Dispose();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Unity;

using NuClear.Assembling.TypeProcessing;

namespace NuClear.Jobs.DI
{
    public class TaskServiceJobsMassProcessor : IMassProcessor
    {
        private readonly IUnityContainer _container;
        private readonly List<Type> _jobTypes = new List<Type>();

        public TaskServiceJobsMassProcessor(IUnityContainer container)
        {
            _container = container;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { typeof(ITaskServiceJob) };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {
                return;
            }

            _jobTypes.AddRange(types.Where(t => !t.IsAbstract));
        }

        public void AfterProcessTypes(bool firstRun)
        {
            if (firstRun)
            {
                return;
            }

            foreach (var jobType in _jobTypes)
            {
                /*
                _container.RegisterTypeWithDependencies(jobType, Lifetime.PerScope, null); 
                 */
            }
        }
    }
}
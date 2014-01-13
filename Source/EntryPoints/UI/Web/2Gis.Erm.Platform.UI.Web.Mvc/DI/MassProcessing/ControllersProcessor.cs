using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.DI.MassProcessing
{
    public class ControllersProcessor : IMassProcessor
    {
        private readonly IUnityContainer _container;
        private readonly List<Type> _ermControllersTypes = new List<Type>();

        public ControllersProcessor(IUnityContainer container)
        {
            _container = container;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { typeof(IController) };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {
                // выполняем проверки при втором проходе
                return;
            }

            _ermControllersTypes.AddRange(types.Where(ShouldBeProcessed));
        }

        public void AfterProcessTypes(bool firstRun)
        {
            if (firstRun)
            {
                // процессинг при втором проходе
                return;
            }

            ProcessControllersForScope(_container, _ermControllersTypes, Mapping.Erm);
        }

        private static bool ShouldBeProcessed(Type type)
        {
            return !type.IsAbstract;
        }

        private static void ProcessControllersForScope(IUnityContainer container, IEnumerable<Type> controllerTypes, string mappingScope)
        {
            foreach (var controllerType in controllerTypes)
            {
                container.RegisterTypeWithDependencies(controllerType, CustomLifetime.PerRequest, mappingScope);
            }
        }
    }
}
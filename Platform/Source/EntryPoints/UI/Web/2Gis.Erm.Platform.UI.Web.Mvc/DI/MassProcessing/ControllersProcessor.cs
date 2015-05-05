using System;
using System.Collections.Generic;
using System.Web.Mvc;

using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

using Microsoft.Practices.Unity;

using NuClear.Assembling.TypeProcessing;
using NuClear.DI.Unity.Config;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.DI.MassProcessing
{
    public class ControllersProcessor : IMassProcessor
    {
        private readonly IUnityContainer _container;
        
        private static readonly Type ErmControllerTypeMarker = typeof(IController);
        private static readonly Type ErmViewModelTypeMarker = typeof(IViewModel);
        
        private readonly List<Type> _ermControllersTypes = new List<Type>();
        private readonly List<Type> _ermViewModelTypes = new List<Type>(); 

        public ControllersProcessor(IUnityContainer container)
        {
            _container = container;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { typeof(IController), typeof(IViewModel) };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {
                // выполняем проверки при втором проходе
                return;
            }

            foreach (var type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                if (ErmControllerTypeMarker.IsAssignableFrom(type))
                {
                    _ermControllersTypes.Add(type);
                }
                else if (ErmViewModelTypeMarker.IsAssignableFrom(type))
                {
                    _ermViewModelTypes.Add(type);
                }
            }
        }

        public void AfterProcessTypes(bool firstRun)
        {
            if (firstRun)
            {
                // процессинг при втором проходе
                return;
            }

            ProcessControllersForScope(_container, _ermControllersTypes, Mapping.Erm);
            _container.RegisterType<IViewModelTypesRegistry, ViewModelTypesRegistry>(Lifetime.Singleton, new InjectionConstructor(_ermViewModelTypes));
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
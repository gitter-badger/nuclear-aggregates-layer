using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DI.Common.Config;
using NuClear.Assembling.TypeProcessing;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services.Enums;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.DI.MassProcessing
{
    /// TODO {all, 24.07.2013}: Попробовать объеденить с UIServiceMassProcessor
    /// Его ответсвенность по факту регистрация тех же IUIService подобных хреней - возможно получитться объеденить с UIServiceMassProcessor
    /// При объединении можно также запилить общий статический класс индикаторов typeof(XXX) для всех маркерных типов расширяющих IUIService (по аналогии с OperationIndicators)
    public sealed class EnumAdaptationMassProcessor : IMassProcessor
    {
        private static readonly Type EnumAdaptationServiceType = typeof(IEnumAdaptationService);

        private readonly IUnityContainer _container;
        private readonly HashSet<Type> _enumAdaptationServiceImplementations = new HashSet<Type>();

        public EnumAdaptationMassProcessor(IUnityContainer container)
        {
            _container = container;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { EnumAdaptationServiceType };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {
                return;
            }

            foreach (var type in types.Where(ShouldBeProcessed))
            {
                _enumAdaptationServiceImplementations.Add(type);
            }
        }

        public void AfterProcessTypes(bool firstRun)
        {
            if (firstRun)
            {
                // процессинг при втором проходе
                return;
            }

            var enumAdaptationServices = new Dictionary<Type, IEnumAdaptationService>();

            foreach (var implementation in _enumAdaptationServiceImplementations.Where(t => !t.IsGenericTypeDefinition).ToArray())
            {
                var enumAdapterInterface =
                    implementation.GetInterfaces().FirstOrDefault(t => EnumAdaptationServiceType.IsAssignableFrom(t) &&
                        EnumAdaptationServiceType != t && t.IsGenericType && !t.IsGenericTypeDefinition);

                if (enumAdapterInterface != null)
                {
                    var enumType = enumAdapterInterface.GetGenericArguments()[0];
                    enumAdaptationServices.Add(enumType, (IEnumAdaptationService)Activator.CreateInstance(implementation));
                }
            }

            _container.RegisterType<IEnumItemsCache, EnumItemsCache>(Lifetime.Singleton, new InjectionConstructor(enumAdaptationServices));
        }

        private static bool ShouldBeProcessed(Type type)
        {
            if (!type.IsClass || type.IsAbstract)
            {
                return false;
            }

            return true;
        }
    }
}
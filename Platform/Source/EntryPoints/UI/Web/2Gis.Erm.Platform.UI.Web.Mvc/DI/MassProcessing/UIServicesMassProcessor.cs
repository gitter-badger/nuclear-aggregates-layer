using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.UI.Web.Mvc.Services;

using Microsoft.Practices.Unity;

using NuClear.Assembling.TypeProcessing;
using NuClear.DI.Unity.Config;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.DI.MassProcessing
{
    /// TODO {all, 15.07.2013}: Данный massprocessor presentation специфичен => ему не место в этой сборке (здесь общий DI функционал для нескольких точек входа разного назначения)
    ///     нужно перенести в сборку MVC приложения, либо common UI (общую для WEB и WPF) сборку
    /// DONE {i.maslennikov 24.07.2013}
    public sealed class UIServicesMassProcessor : IMassProcessor
    {
        private static readonly Type EntityUIServiceType = typeof(IEntityUIService);
        private static readonly Type[] SupportedEntitySpecificUIServiceTypes = { typeof(IEntityUIService<>) };

        private readonly IUnityContainer _container;
        private readonly Func<LifetimeManager> _lifetimeManagerFactoryMethod;
        private readonly string _mappingScope;
        private readonly HashSet<Type> _uiServiceImplementations = new HashSet<Type>();

        public UIServicesMassProcessor(IUnityContainer container, Func<LifetimeManager> lifetimeManagerFactoryMethod, string mappingScope)
        {
            _container = container;
            _lifetimeManagerFactoryMethod = lifetimeManagerFactoryMethod;
            _mappingScope = mappingScope;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { EntityUIServiceType };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {
                return;
            }

            foreach (var type in types.Where(ShouldBeProcessed))
            {
                _uiServiceImplementations.Add(type);
            }
        }

        public void AfterProcessTypes(bool firstRun)
        {
            if (firstRun)
            {
                // процессинг при втором проходе
                return;
            }

            foreach (var implementation in _uiServiceImplementations)
            {
                var implementedInterfacesWithoutUIServiceMarker =
                    implementation.GetInterfaces()
                                  .Where(t => EntityUIServiceType.IsAssignableFrom(t) &&
                                              EntityUIServiceType != t &&
                                              (!t.IsGenericType ||
                                               (t.IsGenericType && !SupportedEntitySpecificUIServiceTypes.Contains(t.GetGenericTypeDefinition()))))
                                  .ToArray();

                foreach (var uiServiceInterface in implementedInterfacesWithoutUIServiceMarker)
                {
                    var typeFrom = uiServiceInterface;

                    var entitySpecificUIServices =
                        uiServiceInterface.GetInterfaces()
                                          .Where(t => EntityUIServiceType.IsAssignableFrom(t) &&
                                                      (t.IsGenericType && SupportedEntitySpecificUIServiceTypes.Contains(t.GetGenericTypeDefinition())))
                                          .ToArray();

                    // processing nongeneric UI service interface
                    if (!entitySpecificUIServices.Any())
                    {
                        continue;
                    }

                    if (entitySpecificUIServices.Count() > 1)
                    {
                        throw new InvalidOperationException("Multiple entity specific UI service implemented interfaces found. Processing interface: " + uiServiceInterface);
                    }

                    if (uiServiceInterface.IsGenericType)
                    {
                        var genericArguments = implementation.GetGenericArguments();
                        if (!uiServiceInterface.IsGenericTypeDefinition && genericArguments.Any(x => x.IsGenericParameter))
                        {   
                            // это open generic
                            // open generics регистрируются как open generic abstraction => open generic implementation
                            typeFrom = uiServiceInterface.GetGenericTypeDefinition();
                        }
                    }

                    _container.RegisterTypeWithDependencies(typeFrom, implementation, _lifetimeManagerFactoryMethod(), _mappingScope);
                }
            }
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

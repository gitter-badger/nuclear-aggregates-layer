using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.DI
{
    public sealed class UnityDependencyResolver : IDependencyResolver
    {
        private readonly IUnityContainer _container;
        
        private readonly IReadOnlyDictionary<Type, Func<IUnityContainer, object>> _overridedMvcSingleRegisteredDependencies;
        private readonly IReadOnlyDictionary<Type, Func<IUnityContainer, IEnumerable<object>>> _overridedMvcMultipleRegisteredDependencies;
        private readonly IGlobalizationSettings _globalizationSettings;

        public UnityDependencyResolver(IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            _globalizationSettings = globalizationSettings;
            _container = container;

            _overridedMvcSingleRegisteredDependencies = new Dictionary<Type, Func<IUnityContainer, object>> 
                                                            {
                                                                { typeof(IControllerActivator), c => c.Resolve<IControllerActivator>() },
                                                                { typeof(IControllerFactory), c => new GenericContollerFactory(_globalizationSettings, c.Resolve<IViewModelTypesRegistry>()) },
                                                                { typeof(ModelMetadataProvider), c => c.Resolve<ModelMetadataProvider>() },
                                                            };

            _overridedMvcMultipleRegisteredDependencies = new Dictionary<Type, Func<IUnityContainer, IEnumerable<object>>>
                                                              {
                                                                  { typeof(IModelBinderProvider), unityContainer => new[] { new ModelBinderProvider() } }
                                                              };
        }

        object IDependencyResolver.GetService(Type serviceType)
        {
            Func<IUnityContainer, object> dependencyFactory;
            if (_overridedMvcSingleRegisteredDependencies.TryGetValue(serviceType, out dependencyFactory))
            {
                return dependencyFactory(_container);
            }

            return null;
        }

        IEnumerable<object> IDependencyResolver.GetServices(Type serviceType)
        {
            Func<IUnityContainer, IEnumerable<object>> dependencyFactory;
            if (_overridedMvcMultipleRegisteredDependencies.TryGetValue(serviceType, out dependencyFactory))
            {
                return dependencyFactory(_container);
            }

            return Enumerable.Empty<object>();
        }
    }
}
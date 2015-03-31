using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.DI.Factories.RequestHandling;
using DoubleGis.Erm.Platform.DI.Common.Config;

using Microsoft.Practices.Unity;

using NuClear.Assembling.TypeProcessing;

namespace DoubleGis.Erm.BLCore.DI.Config.MassProcessing
{
    public class RequestHandlersMassProcessor : IMassProcessor
    {
        private const string ScopeForHandlers = Mapping.Erm;

        private readonly List<Type> _commonRequestHandlersTypes = new List<Type>();
        private readonly Dictionary<Type, HashSet<Type>> _request2RequestHandlerMap = new Dictionary<Type, HashSet<Type>>();
        private readonly IUnityContainer _container;
        private readonly Func<LifetimeManager> _lifetimeManagerFactoryMethod;

        public RequestHandlersMassProcessor(IUnityContainer container, Func<LifetimeManager> lifetimeManagerFactoryMethod)
        {
            _container = container;
            _lifetimeManagerFactoryMethod = lifetimeManagerFactoryMethod;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { typeof(IRequestHandler) };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            if (!firstRun)
            {
                return;
            }

            foreach (var type in types.Where(ShouldBeProcessed))
            {
                if (type.IsGenericType && type.IsGenericTypeDefinition && !type.IsAbstract)
                {
                    _commonRequestHandlersTypes.Add(type);
                }

                var requestType = RequestHandlerUtils.GetRequestType(type);

                HashSet<Type> handlerTypes;
                if (_request2RequestHandlerMap.TryGetValue(requestType, out handlerTypes))
                {
                    handlerTypes.Add(type);
                }
                else
                {
                    _request2RequestHandlerMap.Add(requestType, new HashSet<Type> { type });
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

            var requestHandlerTypes = _request2RequestHandlerMap.Values.SelectMany(x => x).ToArray();
            
            // erm scope handlers
            RegisterScopeHandlers(_container, requestHandlerTypes);
            RegisterScopeHandlers(_container, _commonRequestHandlersTypes);
            RegisterScopeHandlersInfrastructure(_container, requestHandlerTypes.Union(_commonRequestHandlersTypes).ToArray());
        }

        private static bool ShouldBeProcessed(Type type)
        {
            if (type.IsAbstract || (type.BaseType != null && !type.BaseType.IsGenericType))
            {
                return false;
            }

            return true;
        }

        private void RegisterScopeHandlersInfrastructure(IUnityContainer container, Type[] handlerTypes)
        {
            container.RegisterTypeWithDependencies<IRequestHandlerFactory, UnityHandlerFactory>(ScopeForHandlers, _lifetimeManagerFactoryMethod())
                .RegisterType<IRequestHandlerRepository, RequestHandlerRepository>(ScopeForHandlers, Lifetime.Singleton, new InjectionConstructor(handlerTypes, ScopeForHandlers))
                .RegisterTypeWithDependencies<IRequestProcessor, RequestProcessor>(ScopeForHandlers, _lifetimeManagerFactoryMethod())
                .RegisterTypeWithDependencies<ISubRequestProcessor, RequestProcessor>(ScopeForHandlers, _lifetimeManagerFactoryMethod());
        }

        private void RegisterScopeHandlers(IUnityContainer container, IEnumerable<Type> requestHandlers)
        {
            foreach (var requestHandler in requestHandlers)
            {
                container.RegisterTypeWithDependencies(requestHandler, ScopeForHandlers, _lifetimeManagerFactoryMethod());
            }
        }
    }
}
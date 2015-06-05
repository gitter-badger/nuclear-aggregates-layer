using System;

using Microsoft.Practices.Unity;

using NuClear.Storage.Core;
using NuClear.Tracing.API;

namespace NuClear.Aggregates.Storage.DI.Unity
{
    public sealed class UnityAggregateServiceIsolator : IAggregateServiceIsolator
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IDomainContextsScopeFactory _domainContextsScopeFactory;
        private readonly ITracer _tracer;

        public UnityAggregateServiceIsolator(IUnityContainer unityContainer, IDomainContextsScopeFactory domainContextsScopeFactory, ITracer tracer)
        {
            _unityContainer = unityContainer;
            _domainContextsScopeFactory = domainContextsScopeFactory;
            _tracer = tracer;
        }

        public void Execute<TAggregateService>(Action<TAggregateService> action) where TAggregateService : class, IAggregateService
        {
            Func<TAggregateService, bool> ignoreResultFunc = x =>
            {
                action(x);
                return true;
            };

            Execute(ignoreResultFunc);
        }

        public TResult Execute<TAggregateService, TResult>(Func<TAggregateService, TResult> func) where TAggregateService : class, IAggregateService
        {
            try
            {
                using (var scope = _domainContextsScopeFactory.CreateScope())
                {
                    var aggregateServiceFactory = _unityContainer.Resolve<UnityScopedAggregateRepositoryFactory>(new DependencyOverride<IDomainContextHost>(scope));

                    var aggregateServiceType = typeof(TAggregateService);
                    var aggregateService = (TAggregateService)aggregateServiceFactory.CreateRepository(aggregateServiceType);
                    var result = func(aggregateService);

                    scope.Complete();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat(ex, "Isolated aggregate service execution failed");
                throw;
            }
        }
    }
}
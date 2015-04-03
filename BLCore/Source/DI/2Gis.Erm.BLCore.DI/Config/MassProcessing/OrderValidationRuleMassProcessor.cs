using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.DI.Factories.OrderValidation;
using DoubleGis.Erm.Platform.DI.Common.Config;
using NuClear.Assembling.TypeProcessing;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.DI.Config.MassProcessing
{
    public sealed class OrderValidationRuleMassProcessor : IMassProcessor
    {
        private static readonly Type OrderValidationRuleIndicator = typeof(IOrderValidationRule);

        private readonly IUnityContainer _container;
        private readonly Func<LifetimeManager> _lifetimeManagerFactoryMethod;

        public OrderValidationRuleMassProcessor(IUnityContainer container, Func<LifetimeManager> lifetimeManagerFactoryMethod)
        {
            _container = container;
            _lifetimeManagerFactoryMethod = lifetimeManagerFactoryMethod;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { OrderValidationRuleIndicator };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            foreach (var type in types.Where(ShouldBeProcessed))
            {
                _container.RegisterTypeWithDependencies(type, Mapping.OrderValidationRulesScope, _lifetimeManagerFactoryMethod(), Mapping.Erm);                
            }
        }

        public void AfterProcessTypes(bool firstRun)
        {
            if (firstRun)
            {
                return;
            }

            _container.RegisterType<IOrderValidationRuleFactory, UnityOrderValidationRuleFactory>(Mapping.OrderValidationRulesScope, Lifetime.Singleton);
        }

        private static bool ShouldBeProcessed(Type type)
        {
            return !type.IsAbstract;
        }
    }
}
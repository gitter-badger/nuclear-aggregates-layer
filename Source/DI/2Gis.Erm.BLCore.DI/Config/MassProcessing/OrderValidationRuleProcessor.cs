using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Platform.DI.Common.Config.MassProcessing;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.DI.Config.MassProcessing
{
    public class OrderValidationRuleProcessor : IMassProcessor
    {
        private static readonly Type OrderValidationRuleType = typeof(IOrderValidationRule);

        private readonly IUnityContainer _container;
        private readonly Func<LifetimeManager> _lifetimeManagerFactoryMethod;

        public OrderValidationRuleProcessor(IUnityContainer container, Func<LifetimeManager> lifetimeManagerFactoryMethod)
        {
            _container = container;
            _lifetimeManagerFactoryMethod = lifetimeManagerFactoryMethod;
        }

        public Type[] GetAssignableTypes()
        {
            return new[] { OrderValidationRuleType };
        }

        public void ProcessTypes(IEnumerable<Type> types, bool firstRun)
        {
            foreach (var type in types.Where(ShouldBeProcessed))
            {
                _container.RegisterTypeWithDependencies(OrderValidationRuleType, type, type.ToString(), _lifetimeManagerFactoryMethod(), Mapping.Erm);                
            }
        }

        public void AfterProcessTypes(bool firstRun)
        {
            // do nothing
        }

        private static bool ShouldBeProcessed(Type type)
        {
            return !type.IsAbstract;
        }
    }
}
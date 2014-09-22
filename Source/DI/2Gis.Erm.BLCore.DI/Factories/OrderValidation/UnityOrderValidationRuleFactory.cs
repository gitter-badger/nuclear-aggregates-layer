using System;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.Platform.DI.Common.Config;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.DI.Factories.OrderValidation
{
    public sealed class UnityOrderValidationRuleFactory : IOrderValidationRuleFactory
    {
        private readonly IUnityContainer _container;

        public UnityOrderValidationRuleFactory(IUnityContainer container)
        {
            _container = container;
        }

        public IOrderValidationRule Create(Type orderValidationRuleType)
        {
            var orderValidationRuleIndicator = typeof(IOrderValidationRule);
            if (!orderValidationRuleIndicator.IsAssignableFrom(orderValidationRuleType))
            {
                throw new InvalidOperationException("Specified type " + orderValidationRuleType.FullName + " doesn't implement " + orderValidationRuleIndicator.FullName);
            }

            return (IOrderValidationRule)_container.Resolve(orderValidationRuleType, Mapping.OrderValidationRulesScope);
        }
    }
}

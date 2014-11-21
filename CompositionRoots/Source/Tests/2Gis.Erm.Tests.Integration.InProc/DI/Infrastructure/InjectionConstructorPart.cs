using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace DoubleGis.Erm.Tests.Integration.InProc.DI.Infrastructure
{
    /// <summary>
    /// Используется для указания значения некоторых зависимостей типа
    /// Некоторое подобие DependencyOverride, используемое на этапе регистрации
    /// </summary>
    internal class InjectionConstructorPart : InjectionMember
    {
        private readonly Dictionary<Type, object> _overrides;

        public InjectionConstructorPart(params ConstructorParameterOverride[] overrides)
        {
            _overrides = overrides.ToDictionary(x => x.ParameterType, x => x.ParameterValue);
        }

        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            var constructor = implementationType.GetConstructors().Single();

            var parameterValues = constructor.GetParameters()
                                             .Select(x => InjectionParameterValue.ToParameter(OverrideParameter(x.ParameterType)))
                                             .ToArray();

            // Указываем, какой конструктор и какие значения параметров использовать при создании экземпляра типа
            policies.Set<IConstructorSelectorPolicy>(new SpecifiedConstructorSelectorPolicy(constructor, parameterValues),
                                                     new NamedTypeBuildKey(implementationType, name));
        }

        private object OverrideParameter(Type parameterType)
        {
            object value;
            return _overrides.TryGetValue(parameterType, out value) ? value : parameterType;
        }
    }
}
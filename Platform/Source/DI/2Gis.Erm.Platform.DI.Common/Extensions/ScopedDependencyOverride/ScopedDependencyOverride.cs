using System;

using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace DoubleGis.Erm.Platform.DI.Common.Extensions.ScopedDependencyOverride
{
    /// <summary>
    /// Реализует то же поведение, что и обычный DependencyOverride, 
    /// однако, пееопределяет зависимость только при совпадении scope переопределяемой зависимости и scope переданного в конструктор данного класса
    /// </summary>
    public class ScopedDependencyOverride : ResolverOverride
    {
        private readonly InjectionParameterValue _dependencyValue;
        private readonly string _scope;
        private readonly Type _typeToConstruct;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedDependencyOverride"/> class. 
        /// Create an instance of <see cref="DependencyOverride"/> to override
        /// the given type with the given value.
        /// </summary>
        /// <param name="scope">
        /// </param>
        /// <param name="typeToConstruct">
        /// Type of the dependency.
        /// </param>
        /// <param name="dependencyValue">
        /// Value to use.
        /// </param>
        public ScopedDependencyOverride(string scope, Type typeToConstruct, object dependencyValue)
        {
            if (scope == null)
            {
                throw new ArgumentNullException("scope");
            }

            if (typeToConstruct == null)
            {
                throw new ArgumentNullException("typeToConstruct");
            }

            _scope = scope;
            _typeToConstruct = typeToConstruct;
            _dependencyValue = InjectionParameterValue.ToParameter(dependencyValue);
        }

        /// <summary>
        /// Return a <see cref="IDependencyResolverPolicy"/> that can be used to give a value
        /// for the given desired dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="dependencyType">Type of dependency desired.</param>
        /// <returns>a <see cref="IDependencyResolverPolicy"/> object if this override applies, null if not.</returns>
        public override IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
        {
            if (dependencyType != _typeToConstruct)
            {
                return null;
            }

            var operation = context.CurrentOperation as ConstructorArgumentResolveOperation;
            if (operation == null)
            {
                return null;
            }
            
            IPolicyList policyList;
            var policy = context.PersistentPolicies.Get(typeof(IConstructorSelectorPolicy), context.BuildKey, false, out policyList) as SpecifiedConstructorSelectorPolicy;
            if (policy == null)
            {
                var checkingPolicy = context.PersistentPolicies.Get(typeof(IBuildPlanPolicy), context.BuildKey, false, out policyList) as IBuildPlanPolicy;
                if (checkingPolicy != null)
                {
                    // т.е. тип реализующий  IBuildPlanPolicy для указанного BuildKey найден, значит тип либо непосредственно зарегистрирован в контейнере,
                    // либо в автоматическом режиме, например, при resolve open generic регистраций
                    // для запрощенного типа в указанном scope нет IBuilderPolicy реализующей IConstructorSelectorPolicy, 
                    // т.е. никаких спец. настроек для constructor injection не задано (например, через InjectionConstructor и ResolvedParameter) 
                    // => считаем что раз вызвали данный метод GetResolver - то указанная зависимость dependencyType есть среди зависимостей конструируемого типа,
                    // а т.к. явных настроек для constructor injection не задано, то считаем, что переопределять экземпляр зависимости надо 
                    return _dependencyValue.GetResolverPolicy(dependencyType);
                }
                
                return null;
            }

            var constructorParameter = ConstructorParamsExtractor.ExtractConstructorParams.Value(policy, operation.ParameterName) as ResolvedParameter;
            if (constructorParameter == null)
            {
                return null;
            }

            var paramResolverPolicy = constructorParameter.GetResolverPolicy(dependencyType) as NamedTypeDependencyResolverPolicy;
            if (paramResolverPolicy == null)
            {
                return null;
            }

            return string.CompareOrdinal(paramResolverPolicy.Name, _scope) == 0 ? _dependencyValue.GetResolverPolicy(dependencyType) : null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.Unity.Utility;

namespace DoubleGis.Erm.Platform.DI.Common.Extensions
{
    /// <summary>
    /// Этот класс фактически клон InjectionConstructor из Unity, но с изменненной сигнатурой конструктора (удалено params keyword).
    /// Необходимости в этом возникла из-за автоматической настройки mapping'a параметра для некоторых типов
    /// A class that holds the collection of information
    /// for a constructor, so that the container can
    /// be configured to call this constructor.
    /// </summary>
    public class InjectionConstructorEx : InjectionMember
    {
        private readonly List<InjectionParameterValue> _parameterValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectionConstructorEx"/> class. 
        /// Create a new instance of <see cref="InjectionConstructor"/> that looks
        /// for a constructor with the given set of parameters.
        /// </summary>
        /// <param name="parameters">
        /// The values for the parameters, that will
        /// be converted to <see cref="InjectionParameterValue"/> objects.
        /// </param>
        public InjectionConstructorEx(IEnumerable<object> parameters)
        {
            this._parameterValues = parameters.Select(InjectionParameterValue.ToParameter).ToList();
        }

        /// <summary>
        /// Add policies to the <paramref name="policies"/> to configure the
        /// container to call this constructor with the appropriate parameter values.
        /// </summary>
        /// <param name="serviceType">Interface registered, ignored in this implementation.</param>
        /// <param name="implementationType">Type to register.</param>
        /// <param name="name">Name used to resolve the type object.</param>
        /// <param name="policies">Policy list to add policies to.</param>
        public override void AddPolicies(Type serviceType, Type implementationType, string name, IPolicyList policies)
        {
            ConstructorInfo ctor = FindConstructor(implementationType);
            policies.Set<IConstructorSelectorPolicy>(
                new SpecifiedConstructorSelectorPolicy(ctor, this._parameterValues.ToArray()),
                new NamedTypeBuildKey(implementationType, name));
        }

        private ConstructorInfo FindConstructor(Type typeToCreate)
        {
            var matcher = new ParameterMatcher(this._parameterValues);
            foreach (ConstructorInfo ctor in typeToCreate.GetConstructors())
            {
                if (matcher.Matches(ctor.GetParameters()))
                {
                    return ctor;
                }
            }

            throw new InvalidOperationException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "NoSuchConstructor"));
        }
    }
}
